/**
 * ============================================================================
 * ROULETTE GAME JAVASCRIPT
 * ============================================================================
 * 
 * This module handles all client-side functionality for the Roulette game:
 * - Wheel generation and animation
 * - Bet selection and validation
 * - Spin animation and result display
 * - UI state management
 * 
 * UML Class Structure:
 * - Global Variables (State)
 * - Constants (Configuration)
 * - Utility Functions (Audio, Helpers)
 * - Wheel Functions (Generation, Animation)
 * - Betting Functions (Selection, Validation)
 * - Event Handlers (DOM Interaction)
 * ============================================================================
 */

// ============================================================================
// SECTION 1: GLOBAL STATE VARIABLES
// ============================================================================

/** Currently selected bet type (e.g., "red", "black", "1", "1st12") */
let selectedBetType = '';

/** Flag indicating if wheel is currently spinning */
let isSpinning = false;

// ============================================================================
// SECTION 2: CONSTANTS & CONFIGURATION
// ============================================================================

/**
 * American Roulette wheel number order (counter-clockwise from 0)
 * Contains 38 numbers: 0, 00, and 1-36
 */
const wheelNumbers = [
    0, 28, 9, 26, 30, 11, 7, 20, 32, 17, 5, 22, 34, 15, 3, 24, 36, 13, 1,
    '00', 27, 10, 25, 29, 12, 8, 19, 31, 18, 6, 21, 33, 16, 4, 23, 35, 14, 2
];

/**
 * Array of red numbers in American Roulette
 * Used for color determination when creating wheel numbers
 */
const redNumbers = [1, 3, 5, 7, 9, 12, 14, 16, 18, 19, 21, 23, 25, 27, 30, 32, 34, 36];

// ============================================================================
// SECTION 3: UTILITY FUNCTIONS
// ============================================================================

/**
 * Creates a tick sound effect using Web Audio API
 * Used during wheel spinning for audio feedback
 */
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

/**
 * Gets the index of a winner number in the wheelNumbers array
 * @param {number|string} winnerNumber - The winning number (0-36, or '00')
 * @returns {number} Index of the number in wheelNumbers array, or -1 if not found
 */
function getWinnerIndex(winnerNumber) {
    return wheelNumbers.findIndex(num => {
        if (winnerNumber === '00') return num === '00';
        if (winnerNumber === 0) return num === 0;
        return num === winnerNumber;
    });
}

// ============================================================================
// SECTION 4: WHEEL GENERATION & ANIMATION FUNCTIONS
// ============================================================================

/**
 * Creates and positions all wheel numbers dynamically
 * Generates 38 number elements and positions them in a circle
 */
function createWheel() {
    const wheel = document.getElementById('rouletteWheel');
    if (!wheel) return;
    
    // Clear existing wheel numbers
    const existingNumbers = wheel.querySelectorAll('.wheel-number');
    existingNumbers.forEach(el => el.remove());
    
    const totalNumbers = wheelNumbers.length;
    const angleStep = 360 / totalNumbers;
    
    wheelNumbers.forEach((num, index) => {
        const angle = index * angleStep;
        const rad = angle * (Math.PI / 180);
        
        // Create number element
        const numberDiv = document.createElement('div');
        numberDiv.className = 'wheel-number';
        numberDiv.textContent = num === 0 ? '0' : num === '00' ? '00' : num.toString();
        numberDiv.setAttribute('data-number', num);
        
        // Determine color class
        if (num === 0 || num === '00') {
            numberDiv.classList.add('green');
        } else if (redNumbers.includes(num)) {
            numberDiv.classList.add('red');
        } else {
            numberDiv.classList.add('black');
        }
        
        // Calculate position based on wheel size
        const wheelWidth = wheel.offsetWidth || 320;
        const numberSize = wheel.classList.contains('wheel-compact') ? 36 : 48;
        const radius = (wheelWidth / 2) - (numberSize / 2) - 8;
        const x = Math.cos(rad) * radius;
        const y = Math.sin(rad) * radius;
        
        // Position and rotate number
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

/**
 * Animates the wheel spinning to a specific winner number
 * @param {HTMLElement} wheelEl - The wheel DOM element
 * @param {number|string} winnerNumber - The winning number to spin to
 * @param {Object} options - Animation options (duration, etc.)
 */
function spinToWinner(wheelEl, winnerNumber, options = {}) {
    const duration = options.duration || 5000;
    
    const sliceCount = wheelNumbers.length;
    const sliceAngle = 360 / sliceCount;
    const winnerIndex = getWinnerIndex(winnerNumber);
    
    if (winnerIndex === -1) {
        console.error('Winner number not found:', winnerNumber);
        return;
    }
    
    // Calculate winning number position
    const centerOfWinner = winnerIndex * sliceAngle + sliceAngle / 2;
    
    // Get current rotation
    let startAngle = 0;
    const computedStyle = window.getComputedStyle(wheelEl);
    const transform = computedStyle.transform || wheelEl.style.transform || '';
    if (transform && transform !== 'none') {
        const match = transform.match(/rotate\((-?\d+\.?\d*)deg\)/);
        if (match) {
            startAngle = parseFloat(match[1]);
        }
    }
    
    // Calculate final rotation (5-8 extra rotations + position to top)
    const extraRotations = 5 + Math.random() * 3;
    const angleToAdd = extraRotations * 360 + (360 - centerOfWinner);
    const wheelRotation = startAngle + angleToAdd;
    
    // Apply animation
    wheelEl.style.transition = `transform ${duration}ms cubic-bezier(0.22, 0.61, 0.36, 1)`;
    wheelEl.offsetHeight; // Force reflow
    wheelEl.style.transform = `rotate(${wheelRotation}deg)`;
    
    // After animation completes
    setTimeout(() => {
        wheelEl.style.transition = 'none';
        wheelEl.style.transform = `rotate(${wheelRotation}deg)`;
        
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

/**
 * Highlights the winning number on both wheel and table
 * @param {number|string} number - The winning number to highlight
 */
function highlightWinningNumber(number) {
    // Remove previous highlights
    document.querySelectorAll('.wheel-number').forEach(el => {
        el.classList.remove('winning');
    });
    
    // Highlight on wheel
    document.querySelectorAll('.wheel-number').forEach(el => {
        const elNum = el.getAttribute('data-number');
        if (elNum == number || (elNum === '00' && number === '00')) {
            el.classList.add('winning');
        }
    });
    
    // Highlight on table
    if (!isNaN(number) || number === '0' || number === '00') {
        const numStr = number === 0 ? '0' : number === '00' ? '00' : number.toString();
        document.querySelectorAll('.number-chip').forEach(el => {
            const chipBet = el.getAttribute('data-bet');
            if (chipBet === numStr || (chipBet === '0' && number === 0) || (chipBet === '00' && number === '00')) {
                el.classList.add('winning-button');
            } else {
                el.classList.remove('winning-button');
            }
        });
    }
}

/**
 * Resets the wheel to default position and clears highlights
 */
function resetWheel() {
    const wheel = document.getElementById('rouletteWheel');
    if (!wheel) return;
    
    wheel.style.transition = 'none';
    wheel.style.transform = 'rotate(0deg)';
    
    // Remove winning highlights
    document.querySelectorAll('.wheel-number').forEach(el => {
        el.classList.remove('winning');
    });
    document.querySelectorAll('.number-chip').forEach(el => {
        el.classList.remove('winning-button');
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

// ============================================================================
// SECTION 5: BETTING FUNCTIONS
// ============================================================================

/**
 * Selects a bet type and highlights it on the UI
 * @param {string} betType - The bet type to select (e.g., "red", "1", "1st12")
 */
function selectBet(betType) {
    if (isSpinning) return;
    
    selectedBetType = betType;
    const betTypeInput = document.getElementById('selectedBetType');
    if (betTypeInput) {
        betTypeInput.value = betType;
    }
    
    // Remove previous selection
    document.querySelectorAll('.bet-button, .wheel-number, .number-chip, .outside-bet-btn, .dozen-bet, .column-bet-btn').forEach(btn => {
        btn.classList.remove('selected');
    });
    
    // Get clicked element
    let clickedElement = null;
    if (typeof event !== 'undefined' && event.target) {
        clickedElement = event.target.closest('.bet-button, .wheel-number, .number-chip, .outside-bet-btn, .dozen-bet, .column-bet-btn') || event.target;
    } else {
        clickedElement = document.querySelector(`[data-bet="${betType}"], .number-chip[data-bet="${betType}"], .outside-bet-btn[data-bet="${betType}"], .dozen-bet[data-bet="${betType}"], .column-bet-btn[data-bet="${betType}"]`);
    }
    
    if (clickedElement) {
        clickedElement.classList.add('selected');
    }
    
    // Highlight corresponding wheel number if it's a number bet
    if (!isNaN(betType) || betType === '0' || betType === '00') {
        const num = betType === '0' ? 0 : betType === '00' ? '00' : parseInt(betType);
        document.querySelectorAll('.wheel-number').forEach(el => {
            const elNum = el.getAttribute('data-number');
            if (elNum == num || (elNum === '00' && num === '00')) {
                el.classList.add('selected');
            }
        });
    }
    
    // Highlight corresponding table chip if it's a number bet
    if (!isNaN(betType) || betType === '0' || betType === '00') {
        const num = betType === '0' ? 0 : betType === '00' ? '00' : parseInt(betType);
        const numStr = num === 0 ? '0' : num === '00' ? '00' : num.toString();
        document.querySelectorAll('.number-chip').forEach(el => {
            const chipBet = el.getAttribute('data-bet');
            if (chipBet === numStr || (chipBet === '0' && num === 0) || (chipBet === '00' && num === '00')) {
                el.classList.add('selected');
            }
        });
    }
}

/**
 * Validates that a bet has been selected and amount is valid
 * @returns {boolean} True if bet is valid, false otherwise
 */
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

// ============================================================================
// SECTION 6: EVENT HANDLERS & INITIALIZATION
// ============================================================================

/**
 * Initializes the roulette game on page load
 * Sets up event handlers and creates the wheel
 */
document.addEventListener('DOMContentLoaded', function() {
    createWheel();
    
    // Spin form handler (AJAX)
    const spinForm = document.getElementById('spinForm');
    if (spinForm) {
        spinForm.addEventListener('submit', function(e) {
            e.preventDefault();
            
            if (isSpinning) return;
            
            const wheel = document.getElementById('rouletteWheel');
            if (!wheel) return;
            
            const formData = new FormData(spinForm);
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
            
            // Clear previous highlights
            document.querySelectorAll('.wheel-number').forEach(el => {
                el.classList.remove('winning');
            });
            document.querySelectorAll('.number-chip').forEach(el => {
                el.classList.remove('winning-button');
            });
            
            // Update result display
            const resultDisplay = document.getElementById('resultNumberDisplay');
            if (resultDisplay) {
                resultDisplay.textContent = '?';
            }
            
            // Submit via AJAX
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
                
                // Start spin animation
                spinToWinner(wheel, data.winner, {
                    duration: 5000
                });
                
                // Show result message after animation
                setTimeout(() => {
                    const alertDiv = document.querySelector('.alert');
                    if (alertDiv) {
                        alertDiv.textContent = data.message;
                        alertDiv.className = `alert alert-${data.netWinnings > 0 ? 'success' : data.netWinnings < 0 ? 'danger' : 'info'} alert-dismissible fade show`;
                        alertDiv.style.display = 'block';
                    } else {
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
    document.querySelectorAll('.bet-button, .number-chip, .outside-bet-btn, .dozen-bet, .column-bet-btn').forEach(btn => {
        btn.addEventListener('click', function(e) {
            const betType = this.getAttribute('data-bet');
            if (betType) {
                selectBet(betType);
            }
        });
    });
    
    // Highlight winning number on page load if exists
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
                const wheel = document.getElementById('rouletteWheel');
                if (wheel) {
                    const sliceCount = wheelNumbers.length;
                    const sliceAngle = 360 / sliceCount;
                    const winnerIndex = getWinnerIndex(winnerNumber);
                    
                    if (winnerIndex !== -1) {
                        const centerOfWinner = winnerIndex * sliceAngle + sliceAngle / 2;
                        const finalAngle = (360 - centerOfWinner);
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
