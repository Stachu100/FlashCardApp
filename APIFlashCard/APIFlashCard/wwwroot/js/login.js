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