async function login() {
    const password = document.getElementById('password').value;
    const errorMsg = document.getElementById('error-msg');

    if (password === "admin") {
        document.cookie = "admin_logged_in=true; path=/admin";
        window.location.href = "index.html";
    } else {
        errorMsg.textContent = "Wrong password. Try again.";
    }
}