// Roulette Game JavaScript

let selectedBetType = '';
let isSpinning = false;

// American Roulette wheel order (counter-clockwise from 0)
const wheelNumbers = [
    0, 28, 9, 26, 30, 11, 7, 20, 32, 17, 5, 22, 34, 15, 3, 24, 36, 13, 1,
    '00', 27, 10, 25, 29, 12, 8, 19, 31, 18, 6, 21, 33, 16, 4, 23, 35, 14, 2
];

const redNumbers = [1, 3, 5, 7, 9, 12, 14, 16, 18, 19, 21, 23, 25, 27, 30, 32, 34, 36];

// Create tick sound effect using Web Audio API
function playTickSound() {
    try {
        const audioContext = new (window.AudioContext || window.webkitAudioContext)();
        const oscillator = audioContext.createOscillator();
        const gainNode = audioContext.createGain();
        
        oscillator.connect(gainNode);
        gainNode.connect(audioContext.destination);
        
        oscillator.frequency.value = 800; // Higher pitch for tick
        oscillator.type = 'sine';
        
        gainNode.gain.setValueAtTime(0.1, audioContext.currentTime);
        gainNode.gain.exponentialRampToValueAtTime(0.01, audioContext.currentTime + 0.05);
        
        oscillator.start(audioContext.currentTime);
        oscillator.stop(audioContext.currentTime + 0.05);
    } catch (e) {
        // Fallback: silent tick if audio context fails
        console.log('Tick');
    }
}

function createWheel() {
    const wheel = document.getElementById('rouletteWheel');
    if (!wheel) return;
    
    // Preserve the indicator element - only remove wheel numbers
    // Clear only the wheel numbers, not the indicator
    const existingNumbers = wheel.querySelectorAll('.wheel-number');
    existingNumbers.forEach(el => el.remove());
    
    const totalNumbers = wheelNumbers.length;
    const angleStep = 360 / totalNumbers;
    
    wheelNumbers.forEach((num, index) => {
        const angle = index * angleStep;
        const rad = angle * (Math.PI / 180);
        
        const numberDiv = document.createElement('div');
        numberDiv.className = 'wheel-number';
        numberDiv.textContent = num === 0 ? '0' : num === '00' ? '00' : num.toString();
        numberDiv.setAttribute('data-number', num);
        
        // Determine color
        if (num === 0 || num === '00') {
            numberDiv.classList.add('green');
        } else if (redNumbers.includes(num)) {
            numberDiv.classList.add('red');
        } else {
            numberDiv.classList.add('black');
        }
        
        // Position the number
        const radius = 160; // Half of wheel width minus number size
        const x = Math.cos(rad) * radius;
        const y = Math.sin(rad) * radius;
        
        numberDiv.style.left = `calc(50% + ${x}px)`;
        numberDiv.style.top = `calc(50% + ${y}px)`;
        numberDiv.style.transform = `translate(-50%, -50%) rotate(${angle + 90}deg)`;
        
        // Add click handler
        numberDiv.addEventListener('click', function() {
            selectBet(num.toString());
        });
        
        wheel.appendChild(numberDiv);
    });
}

function highlightWinningNumber(number) {
    document.querySelectorAll('.wheel-number').forEach(el => {
        el.classList.remove('winning');
        const elNum = el.getAttribute('data-number');
        if (elNum == number || (elNum === '00' && number === '00')) {
            el.classList.add('winning');
        }
    });
}

function selectBet(betType) {
    if (isSpinning) return;
    
    selectedBetType = betType;
    const betTypeInput = document.getElementById('selectedBetType');
    if (betTypeInput) {
        betTypeInput.value = betType;
    }
    
    // Remove previous selection
    document.querySelectorAll('.bet-button, .wheel-number').forEach(btn => {
        btn.classList.remove('selected');
    });
    
    // Add selection to clicked button
    const clickedElement = event.target;
    clickedElement.classList.add('selected');
    
    // Also highlight corresponding wheel number if it's a number bet
    if (!isNaN(betType) || betType === '0' || betType === '00') {
        const num = betType === '0' ? 0 : betType === '00' ? '00' : parseInt(betType);
        document.querySelectorAll('.wheel-number').forEach(el => {
            const elNum = el.getAttribute('data-number');
            if (elNum == num || (elNum === '00' && num === '00')) {
                el.classList.add('selected');
            }
        });
    }
}

function validateBet() {
    if (!selectedBetType) {
        alert('Please select a betting area first by clicking on a number or betting option.');
        return false;
    }
    
    const betAmount = document.querySelector('input[name="betAmount"]');
    if (!betAmount || !betAmount.value || betAmount.value <= 0) {
        alert('Please enter a valid bet amount.');
        return false;
    }
    
    return true;
}

// Reset wheel to default position
function resetWheel() {
    const wheel = document.getElementById('rouletteWheel');
    if (!wheel) return;
    
    // Remove any transitions
    wheel.style.transition = 'none';
    
    // Reset wheel rotation to 0 (default position)
    wheel.style.transform = 'rotate(0deg)';
    
    // Remove all winning highlights
    document.querySelectorAll('.wheel-number').forEach(el => {
        el.classList.remove('winning');
    });
    
    // Reset result display
    const resultDisplay = document.getElementById('resultNumberDisplay');
    if (resultDisplay) {
        const resultSpan = resultDisplay.querySelector('span');
        if (resultSpan) {
            resultSpan.textContent = '-';
        } else {
            resultDisplay.textContent = '-';
        }
    }
}

// Get winner index from wheel number
function getWinnerIndex(winnerNumber) {
    return wheelNumbers.findIndex(num => {
        if (winnerNumber === '00') return num === '00';
        if (winnerNumber === 0) return num === 0;
        return num === winnerNumber;
    });
}

// Simple spin animation - wheel spins to winner
function spinToWinner(wheelEl, winnerNumber, options = {}) {
    const duration = options.duration || 5000; // 5 seconds like reference
    
    const sliceCount = wheelNumbers.length;
    const sliceAngle = 360 / sliceCount;
    const winnerIndex = getWinnerIndex(winnerNumber);
    
    if (winnerIndex === -1) {
        console.error('Winner number not found:', winnerNumber);
        return;
    }
    
    // Calculate where the winning number is positioned on the wheel
    const centerOfWinner = winnerIndex * sliceAngle + sliceAngle / 2;
    
    // Get current rotation if any
    let startAngle = 0;
    const computedStyle = window.getComputedStyle(wheelEl);
    const transform = computedStyle.transform || wheelEl.style.transform || '';
    if (transform && transform !== 'none') {
        const match = transform.match(/rotate\((-?\d+\.?\d*)deg\)/);
        if (match) {
            startAngle = parseFloat(match[1]);
        }
    }
    
    // Calculate wheel rotation to bring winner to top
    // We need to rotate the wheel so the winner ends up at the top (0 degrees)
    const extraRotations = 5 + Math.random() * 3; // 5-8 full rotations
    const angleToAdd = extraRotations * 360 + (360 - centerOfWinner);
    const wheelRotation = startAngle + angleToAdd;
    
    // Ensure wheel has transition enabled
    wheelEl.style.transition = `transform ${duration}ms cubic-bezier(0.22, 0.61, 0.36, 1)`;
    
    // Force a reflow to ensure transition is applied
    wheelEl.offsetHeight;
    
    // Apply rotation to wheel
    wheelEl.style.transform = `rotate(${wheelRotation}deg)`;
    
    // After animation completes
    setTimeout(() => {
        // Disable transitions for final positioning
        wheelEl.style.transition = 'none';
        
        // Set final wheel position (brings winner to top)
        wheelEl.style.transform = `rotate(${wheelRotation}deg)`;
        
        // Highlight winning number
        highlightWinningNumber(winnerNumber);
        
        // Update result display
        const resultDisplay = document.getElementById('resultNumberDisplay');
        if (resultDisplay) {
            const resultSpan = resultDisplay.querySelector('span');
            if (resultSpan) {
                resultSpan.textContent = winnerNumber === '00' ? '00' : winnerNumber.toString();
            } else {
                resultDisplay.textContent = winnerNumber === '00' ? '00' : winnerNumber.toString();
            }
        }
        
        // Re-enable spin button
        const spinButton = document.getElementById('spinButton');
        if (spinButton) {
            spinButton.disabled = false;
            const spinText = spinButton.querySelector('.spin-text');
            if (spinText) {
                spinText.textContent = 'SPIN';
            }
        }
        
        isSpinning = false;
    }, duration);
}

// Initialize on page load
document.addEventListener('DOMContentLoaded', function() {
    createWheel();
    
    // Add spin handler to form - use AJAX to get winner first
    const spinForm = document.getElementById('spinForm');
    if (spinForm) {
        spinForm.addEventListener('submit', function(e) {
            e.preventDefault(); // Prevent default form submission
            
            if (isSpinning) return;
            
            const wheel = document.getElementById('rouletteWheel');
            if (!wheel) return;
            
            // Get form data
            const formData = new FormData(spinForm);
            
            // Start spinning state
            isSpinning = true;
            
            // Disable spin button
            const spinButton = document.getElementById('spinButton');
            if (spinButton) {
                spinButton.disabled = true;
                const spinText = spinButton.querySelector('.spin-text');
                if (spinText) {
                    spinText.textContent = 'SPINNING...';
                }
            }
            
            // Remove previous winning highlight
            document.querySelectorAll('.wheel-number').forEach(el => {
                el.classList.remove('winning');
            });
            
            // Update result display
            const resultDisplay = document.getElementById('resultNumberDisplay');
            if (resultDisplay) {
                resultDisplay.textContent = '?';
            }
            
            // Submit via AJAX to get the result
            fetch(spinForm.action, {
                method: 'POST',
                body: formData,
                headers: {
                    'X-Requested-With': 'XMLHttpRequest'
                }
            })
            .then(response => {
                if (!response.ok) {
                    throw new Error('Network response was not ok');
                }
                return response.json();
            })
            .then(data => {
                if (!data.success) {
                    alert(data.message || 'An error occurred. Please try again.');
                    isSpinning = false;
                    if (spinButton) {
                        spinButton.disabled = false;
                        const spinText = spinButton.querySelector('.spin-text');
                        if (spinText) {
                            spinText.textContent = 'SPIN';
                        }
                    }
                    return;
                }
                
                // Start spin animation to the predetermined winner
                spinToWinner(wheel, data.winner, {
                    duration: 5000  // 5 seconds like reference
                });
                
                // Show result message after animation
                setTimeout(() => {
                    // Update the alert message if it exists
                    const alertDiv = document.querySelector('.alert');
                    if (alertDiv) {
                        alertDiv.textContent = data.message;
                        alertDiv.className = `alert alert-${data.netWinnings > 0 ? 'success' : data.netWinnings < 0 ? 'danger' : 'info'} alert-dismissible fade show`;
                        alertDiv.style.display = 'block';
                    } else {
                        // Create alert if it doesn't exist
                        const container = document.querySelector('.roulette-container .container-fluid');
                        if (container) {
                            const alert = document.createElement('div');
                            alert.className = `alert alert-${data.netWinnings > 0 ? 'success' : data.netWinnings < 0 ? 'danger' : 'info'} alert-dismissible fade show`;
                            alert.innerHTML = `
                                ${data.message}
                                <button type="button" class="btn-close" data-bs-dismiss="alert"></button>
                            `;
                            container.insertBefore(alert, container.firstChild.nextSibling);
                        }
                    }
                }, 5000);
            })
            .catch(error => {
                console.error('Error:', error);
                alert('An error occurred. Please try again.');
                isSpinning = false;
                const spinButton = document.getElementById('spinButton');
                if (spinButton) {
                    spinButton.disabled = false;
                    const spinText = spinButton.querySelector('.spin-text');
                    if (spinText) {
                        spinText.textContent = 'SPIN';
                    }
                }
            });
        });
    }
    
    // Add click handlers to all betting areas
    document.querySelectorAll('.bet-button').forEach(btn => {
        btn.addEventListener('click', function(e) {
            const betType = this.getAttribute('data-bet');
            if (betType) {
                selectBet(betType);
            }
        });
    });
    
    // Highlight winning number if exists on page load
    // Also position the wheel to show the winner without animation
    const lastResultElement = document.querySelector('.result-number-display');
    if (lastResultElement) {
        const resultText = lastResultElement.textContent.trim();
        if (resultText && resultText !== '-' && resultText !== '?') {
            let winnerNumber = null;
            if (resultText === '00') {
                winnerNumber = '00';
            } else if (resultText === '0') {
                winnerNumber = 0;
            } else {
                const num = parseInt(resultText);
                if (!isNaN(num)) {
                    winnerNumber = num;
                }
            }
            
            if (winnerNumber !== null) {
                // Position wheel to show winner immediately (no animation)
                const wheel = document.getElementById('rouletteWheel');
                
                if (wheel) {
                    // Calculate final position for the winner
                    const sliceCount = wheelNumbers.length;
                    const sliceAngle = 360 / sliceCount;
                    const winnerIndex = getWinnerIndex(winnerNumber);
                    
                    if (winnerIndex !== -1) {
                        const centerOfWinner = winnerIndex * sliceAngle + sliceAngle / 2;
                        const finalAngle = (360 - centerOfWinner);
                        
                        // Set position immediately without transition
                        // Position wheel so winner is at top
                        wheel.style.transition = 'none';
                        wheel.style.transform = `rotate(${finalAngle}deg)`;
                    }
                }
                
                setTimeout(() => {
                    highlightWinningNumber(winnerNumber);
                }, 100);
            }
        }
    }
});
