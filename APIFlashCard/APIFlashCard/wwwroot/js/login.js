async function login() {
    const username = document.getElementById('username').value;
    const password = document.getElementById('password').value;
    const errorMsg = document.getElementById('error-msg');

    errorMsg.textContent = "";

    if (!username || !password) {
        errorMsg.textContent = "Please provide username and password.";
        return;
    }

    try {
        const response = await fetch('/api/adminpanellogin/login', {
            method: 'POST',
            headers: { 'Content-Type': 'application/json' },
            body: JSON.stringify({ username, password })
        });

        const data = await response.json();

        if (!response.ok) {
            errorMsg.textContent = data.message || "Login failed.";
            return;
        }

        document.cookie = "admin_logged_in=true; path=/; Secure; SameSite=Lax";

        window.location.href = "index.html";
    }

    catch (err) {
        errorMsg.textContent = "Login failed: " + err.message;
    }
}

async function startQrLogin() {
    const status = document.getElementById('qr-status');
    const qrCanvas = document.getElementById('qrCanvas');

    status.textContent = "Oczekiwanie na potwierdzenie QR...";

    try {
        const response = await fetch('/api/qrlogin/generate', {
            method: 'POST',
            headers: { 'Content-Type': 'application/json' }
        });

        const data = await response.json();

        if (!response.ok || !data.token) {
            status.textContent = data.message || "Błąd generowania tokena";
            return;
        }

        const qrToken = data.token;

        QRCode.toCanvas(qrCanvas, qrToken, function (error) {
            if (error) {
                console.error(error);
                status.textContent = "Błąd generowania obrazu QR";
                return;
            }
            status.textContent = "Zeskanuj ten kod QR w aplikacji (ważny 2 minuty)";
        });

        const interval = setInterval(async () => {
            const verifyResp = await fetch('/api/qrlogin/check', {
                method: 'POST',
                headers: { 'Content-Type': 'application/json' },
                body: JSON.stringify({ token: qrToken })
            });

            const verifyData = await verifyResp.json();

            if (verifyResp.ok && verifyData.success) {
                clearInterval(interval);
                status.textContent = `Zalogowano: ${verifyData.username}`;
                await delay(3000);
                window.location.href = "index.html";
            }
        }, 1000);

    } catch (err) {
        status.textContent = "Błąd: " + err.message;
    }
}

function delay(ms) {
    return new Promise(resolve => setTimeout(resolve, ms));
}