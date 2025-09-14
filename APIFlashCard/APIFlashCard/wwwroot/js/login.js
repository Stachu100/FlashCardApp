async function login() {
    const password = document.getElementById('password').value;
    if (password === "admin") {
        document.cookie = "admin_logged_in=true; path=/admin";
        window.location.href = "index.html";
    } else {
        alert("Wrong password");
    }
}