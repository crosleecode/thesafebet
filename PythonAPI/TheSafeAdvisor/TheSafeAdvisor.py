from collections import defaultdict
from fastapi import FastAPI
from pydantic import BaseModel
from starlette.middleware.cors import CORSMiddleware
import random
import math
import pickle

ACTIONS = ["Hit", "Stand"]

app = FastAPI(title="TheSafeAdvisor")

app.add_middleware(
    CORSMiddleware,
    allow_origins=["*"],
)

def deal():
    card = random.randint(1,13)
    if card == 1:
        return 11
    if card >= 10:
        return 10
    return card

def addHand(cards):  #Sum up the hand, used for decision making
    hand = sum(cards)
    aces = cards.count(11)
    while hand > 21 and aces > 0: #convert aces to 1s if they would cause a bust
        hand -= 10
        aces -= 1
    return hand

def playDealerTurn(dealerCard):  #plays the dealer's turn.  if their card total is 16 or below, they always hit
    cards = [dealerCard, deal()]
    while addHand(cards) < 17:
        cards.append(deal())
    return cards

def dealInitial():  #initialize the game, player gets 2 cards, dealer gets 1
    playerCards = [deal(), deal()]
    dealerCard = deal()
    return playerCards, dealerCard

def valueHand(cards):
    hand = sum(cards)
    aces = cards.count(11)
    while hand > 21 and aces > 0:
        hand -= 10
        aces -= 1
    usableFlag = 1 if (11 in cards and hand <= 21) else 0 #checks for if there is a usable ace in the hand
    return hand, usableFlag

def buildState(playerCards, dealerCard):
    hand, usableFlag = valueHand(playerCards)
    return (hand, usableFlag, dealerCard)

def step(state, action, playerCards, dealerCard):
    playerHand, usableFlag, dealerUp = state

    if action == "Stand":
        dealerCards = playDealerTurn(dealerUp)
        player = addHand(playerCards)
        dealer = addHand(dealerCards)

        #print("Final Player: ", playerCards, " Final Dealer: ", dealerCards)
        
        if(player > 21):
            return (None, -1, True)
        if(dealer > 21):
            return (None, +1, True)
        if(player > dealer):
            return (None, +1, True)
        if(player < dealer):
            return (None, -1, True)
        return (None, 0, True)

    playerCards.append(deal())
    player = addHand(playerCards)
    if(player > 21):
        #print("player busted")
        return (None, -1, True)

    nextState = buildState(playerCards, dealerCard)
    return (nextState, 0, False)

def playRound():
    playerCards, dealerCard = dealInitial()
    state = buildState(playerCards, dealerCard)

    while True:
        action = "Hit" if random.random() < 0.5 else "Stand"
        #print(action)
        nextState, reward, doneFlag = step(state, action, playerCards, dealerCard)
        if doneFlag:
            return reward
        state = nextState

def trainQ(rounds, alpha=0.1, gamma=.99, epsStart=1.0, epsEnd=.05):
    Q = {}
    counter = defaultdict(int)

    for total in range(4, 22):
        for ace in [0, 1]:
            for dealer in range(2, 12):
                Q[(total, ace, dealer)] = {"Hit": 0.0, "Stand": 0.0}

    for roundNum in range(rounds):
        eps = epsEnd + (epsStart - epsEnd) * math.exp(-4 * roundNum / rounds)

        playerCards, dealerCard = dealInitial()
        state = buildState(playerCards, dealerCard)

        while True:
            action = random.choice(ACTIONS) if random.random() < eps else max(Q[state], key=Q[state].get)
            nextState, reward, doneFlag = step(state, action, playerCards, dealerCard)

            stateAction = (state, action)
            a = alpha / (1 + counter[stateAction] // 50)

            if doneFlag:
                Q[state][action] += a * (reward - Q[state][action])
                counter[stateAction] += 1
                break
            else:
                Q[state][action] += a * (reward + gamma * max(Q[nextState].values()) - Q[state][action])
                counter[stateAction] += 1
                state = nextState

    return Q

def best_action(Q, s):
    return max(Q.get(s, {"Hit":0,"Stand":0}), key=Q.get(s, {"Hit":0,"Stand":0}).get)

def evalPolicy(Q, rounds=10000):
    wins=losses=pushes=0
    for _ in range(rounds):
        playerCards, dealerCard = dealInitial()
        state = buildState(playerCards, dealerCard)
        while True:
            action = max(Q[state], key=Q[state].get) if state in Q else ("Stand" if state[0]>=17 else "Hit")
            nextState, reward, done = step(state, action, playerCards, dealerCard)
            if done:
                if reward>0: wins+=1
                elif reward<0: losses+=1
                else: pushes+=1
                break
            state = nextState
    n = rounds
    print("greedy policy result:\n wins:", wins," losses:", losses, " pushes:", pushes,"\n")

def evalRandom(rounds=10000):
    wins=losses=pushes=0
    for _ in range(rounds):
        r = playRound()  
        if r>0: wins+=1
        elif r<0: losses+=1
        else: pushes+=1
    n = rounds
    print("random policy result:\n wins:", wins," losses:", losses, " pushes:", pushes,"\n")

class AdviceReq(BaseModel):
    player_total: int
    dealer_upcard: int
    usable_ace: int = 0

Q_GLOBAL = None

@app.on_event("startup")
def startup():
    #global Q_GLOBAL
    #random.seed(0)
    #Q_GLOBAL = trainQ(rounds=1000000)

    global Q_GLOBAL
    random.seed(0)
    with open("q_table.pkl", "rb") as f:
        Q_GLOBAL = pickle.load(f)

@app.get("/health")
def health():
    return {"ok": True}

@app.post("/advise")
def advise(req : AdviceReq):
    s = (req.player_total, req.usable_ace, req.dealer_upcard)

    if s not in Q_GLOBAL:
        action = "Stand" if req.player_total >= 17 else "Hit"
        return {"advice": action, "q": Q_GLOBAL.get(s, {"Hit": 0.0, "Stand": 0.0}), "state": list(s), "source": "fallback"}

    action = max(Q_GLOBAL[s], key=Q_GLOBAL[s].get)
    return {"advice": action, "q": Q_GLOBAL[s], "state": list(s), "source": "q-table"}

if __name__ == "__main__":
    
    random.seed(0)
    Q = trainQ(rounds=1000000)

    with open("q_table.pkl", "wb") as f:
        pickle.dump(Q, f)

    print("Q-table with ", len(Q), " states")

    #checks = [(12,0,2),(12,0,7),(16,0,10),(18,1,6),(13,0,6),(11,0,10),(20,1,5)]
    #for i in checks:
    #    print(i, "->", Q[i], "best:", best_action(Q, i))

    #evalRandom()
    #evalPolicy(Q)

print("FastAPI app initialized successfully")

