async function login() {
    const username = document.getElementById('username').value;
    const password = document.getElementById('password').value;
    const errorMsg = document.getElementById('error-msg');

    errorMsg.textContent = "";

    if (!username || !password) {
        errorMsg.textContent = "Please provide username and password";
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
            errorMsg.textContent = data.message || "Login failed";
            return;
        }

        if (response.ok) {
            document.cookie = "admin_logged_in=true; path=/; Secure; SameSite=Lax";
            document.cookie = `admin_username=${encodeURIComponent(username)}; path=/; Secure; SameSite=Lax`;
            window.location.href = "index.html";
        }
    }

    catch (err) {
        errorMsg.textContent = "Login failed";
    }
}

async function startQrLogin() {
    const status = document.getElementById('qr-status');
    const qrCanvas = document.getElementById('qrCanvas');

    status.textContent = "Waiting for QR confirmation...";

    try {
        const response = await fetch('/api/qrlogin/generate', {
            method: 'POST',
            headers: { 'Content-Type': 'application/json' }
        });

        const data = await response.json();

        if (!response.ok || !data.token) {
            status.textContent = data.message || "Error generating";
            return;
        }

        const qrToken = data.token;

        QRCode.toCanvas(qrCanvas, qrToken, function (error) {
            if (error) {
                console.error(error);
                status.textContent = "Error generating QR image";
                return;
            }
            status.textContent = "Scan this QR code in your app (valid for 2 minutes)";
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
                document.cookie = "admin_logged_in=true; path=/; Secure; SameSite=Lax";
                document.cookie = `admin_username=${encodeURIComponent(verifyData.username)}; path=/; Secure; SameSite=Lax`;
                status.textContent = `Logged in: ${verifyData.username}`;
                await delay(3000);
                window.location.href = "index.html";
            }
        }, 1000);

    } catch (err) {
        status.textContent = "Unexpected error";
    }
}

function delay(ms) {
    return new Promise(resolve => setTimeout(resolve, ms));
}