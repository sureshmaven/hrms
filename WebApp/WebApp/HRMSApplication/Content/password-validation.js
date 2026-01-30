/**
 * Password Validation and Strength Indicator
 * Provides real-time password validation and strength scoring
 */

(function () {
    'use strict';

    // Password strength meter colors
    const strengthColors = {
        weak: '#dc3545',
        medium: '#ffc107',
        strong: '#28a745',
        veryStrong: '#20c997'
    };

    // Initialize password validation for all password fields
    function initializePasswordValidation() {
        const passwordFields = document.querySelectorAll('input[type="password"]');
        passwordFields.forEach(field => {
            setupPasswordField(field);
        });
    }

    // Setup individual password field with validation
    function setupPasswordField(passwordField) {
        // Create strength meter
        const strengthMeter = createStrengthMeter();
        passwordField.parentNode.insertBefore(strengthMeter, passwordField.nextSibling);

        // Create validation message container
        const validationMessage = createValidationMessage();
        strengthMeter.parentNode.insertBefore(validationMessage, strengthMeter.nextSibling);

        // Create generate password button
        const generateBtn = createGenerateButton();
        validationMessage.parentNode.insertBefore(generateBtn, validationMessage.nextSibling);

        // Add event listeners
        passwordField.addEventListener('input', function () {
            validatePasswordRealTime(this.value, strengthMeter, validationMessage);
        });

        passwordField.addEventListener('blur', function () {
            validatePasswordServer(this.value, validationMessage);
        });

        generateBtn.addEventListener('click', function () {
            generateStrongPassword(passwordField);
        });
    }

    // Create strength meter element
    function createStrengthMeter() {
        const container = document.createElement('div');
        container.className = 'password-strength-meter';
        container.innerHTML = `
            <div class="strength-bar">
                <div class="strength-fill" style="width: 0%; background-color: #e9ecef;"></div>
            </div>
            <div class="strength-text">Password strength: <span class="strength-label">Enter password</span></div>
        `;
        return container;
    }

    // Create validation message container
    function createValidationMessage() {
        const container = document.createElement('div');
        container.className = 'password-validation-message';
        container.style.display = 'none';
        return container;
    }

    // Create generate password button
    function createGenerateButton() {
        const button = document.createElement('button');
        button.type = 'button';
        button.className = 'btn btn-outline-secondary btn-sm';
        button.innerHTML = '<i class="fa fa-key"></i> Generate Strong Password';
        button.style.marginTop = '5px';
        return button;
    }

    // Real-time password validation
    function validatePasswordRealTime(password, strengthMeter, validationMessage) {
        if (!password) {
            updateStrengthMeter(strengthMeter, 0, 'Enter password', '#e9ecef');
            validationMessage.style.display = 'none';
            return;
        }

        const score = calculatePasswordScore(password);
        const { label, color } = getStrengthLabel(score);

        updateStrengthMeter(strengthMeter, score, label, color);

        // Show basic validation messages
        const messages = [];
        if (password.length < 8) messages.push('At least 8 characters');
        if (!/[A-Z]/.test(password)) messages.push('One uppercase letter');
        if (!/[a-z]/.test(password)) messages.push('One lowercase letter');
        if (!/\d/.test(password)) messages.push('One number');
        if (!/[^A-Za-z0-9]/.test(password)) messages.push('One special character');

        if (messages.length > 0) {
            validationMessage.innerHTML = '<div class="text-warning"><small>Requirements: ' + messages.join(', ') + '</small></div>';
            validationMessage.style.display = 'block';
        } else {
            validationMessage.innerHTML = '<div class="text-success"><small>✓ Password meets all requirements</small></div>';
            validationMessage.style.display = 'block';
        }
    }

    // Server-side password validation
    function validatePasswordServer(password, validationMessage) {
        if (!password) return;

        fetch('/Register/ValidatePassword', {
            method: 'POST',
            headers: {
                'Content-Type': 'application/json',
                'RequestVerificationToken': getAntiForgeryToken()
            },
            body: JSON.stringify({ password: password })
        })
            .then(response => response.json())
            .then(data => {
                if (!data.isValid) {
                    validationMessage.innerHTML = `
                    <div class="text-danger">
                        <small>${data.message}</small>
                        ${data.suggestion ? `<br><small>Try: <code>${data.suggestion}</code></small>` : ''}
                    </div>
                `;
                    validationMessage.style.display = 'block';
                }
            })
            .catch(error => {
                console.error('Password validation error:', error);
            });
    }

    // Generate strong password
    function generateStrongPassword(passwordField) {
        fetch('/Register/GeneratePassword', {
            method: 'POST',
            headers: {
                'Content-Type': 'application/json',
                'RequestVerificationToken': getAntiForgeryToken()
            }
        })
            .then(response => response.json())
            .then(data => {
                passwordField.value = data.password;
                passwordField.dispatchEvent(new Event('input'));
            })
            .catch(error => {
                console.error('Password generation error:', error);
            });
    }

    // Calculate password strength score (0-100)
    function calculatePasswordScore(password) {
        let score = 0;

        // Length contribution (up to 25 points)
        if (password.length >= 8) score += 10;
        if (password.length >= 10) score += 10;
        if (password.length >= 12) score += 5;

        // Character variety contribution (up to 40 points)
        if (/[A-Z]/.test(password)) score += 10;
        if (/[a-z]/.test(password)) score += 10;
        if (/\d/.test(password)) score += 10;
        if (/[^A-Za-z0-9]/.test(password)) score += 10;

        // Complexity contribution (up to 35 points)
        const uniqueChars = new Set(password).size;
        if (uniqueChars >= 8) score += 10;
        if (uniqueChars >= 10) score += 10;
        if (uniqueChars >= 12) score += 15;

        return Math.min(score, 100);
    }

    // Get strength label and color based on score
    function getStrengthLabel(score) {
        if (score < 25) return { label: 'Very Weak', color: strengthColors.weak };
        if (score < 50) return { label: 'Weak', color: strengthColors.weak };
        if (score < 75) return { label: 'Medium', color: strengthColors.medium };
        if (score < 90) return { label: 'Strong', color: strengthColors.strong };
        return { label: 'Very Strong', color: strengthColors.veryStrong };
    }

    // Update strength meter display
    function updateStrengthMeter(meter, score, label, color) {
        const fill = meter.querySelector('.strength-fill');
        const labelElement = meter.querySelector('.strength-label');

        fill.style.width = score + '%';
        fill.style.backgroundColor = color;
        labelElement.textContent = label;
    }

    // Get anti-forgery token for AJAX requests
    function getAntiForgeryToken() {
        const tokenElement = document.querySelector('input[name="__RequestVerificationToken"]');
        return tokenElement ? tokenElement.value : '';
    }

    // Initialize when DOM is ready
    if (document.readyState === 'loading') {
        document.addEventListener('DOMContentLoaded', initializePasswordValidation);
    } else {
        initializePasswordValidation();
    }

    // Export functions for external use
    window.PasswordValidation = {
        initialize: initializePasswordValidation,
        validate: validatePasswordRealTime,
        generate: generateStrongPassword
    };

})();
