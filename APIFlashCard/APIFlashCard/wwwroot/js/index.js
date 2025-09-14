async function loadUsers() {
    const res = await fetch('/admin/users');
    const users = await res.json();
    const ul = document.getElementById('users');
    ul.innerHTML = '';
    users.forEach(u => {
        const li = document.createElement('li');
        li.textContent = `${u.iD_User} ${u.userName}`;
        ul.appendChild(li);
    });
}

async function loadLogs() {
    const res = await fetch('/admin/logs');
    const logs = await res.json();
    const ul = document.getElementById('logs');
    ul.innerHTML = '';
    logs.forEach(l => {
        const li = document.createElement('li');
        li.textContent = `[${new Date(l.timeStamp).toLocaleString()}] (${l.level}) ${l.message}`;
        ul.appendChild(li);
    });
}

async function loadCategories() {
    const res = await fetch('/admin/categories');
    const categories = await res.json();
    const ul = document.getElementById('categories');
    ul.innerHTML = '';
    categories.forEach(c => {
        const li = document.createElement('li');
        li.textContent = `${c.iD_Category} ${c.categoryName} (${c.frontLanguage} → ${c.backLanguage})`;
        li.style.cursor = 'pointer';
        li.onclick = () => loadFlashCards(c.iD_Category, c.categoryName);
        ul.appendChild(li);
    });
}

async function addCategory() {
    const name = document.getElementById('categoryName').value;
    const frontLang = document.getElementById('frontLang').value;
    const backLang = document.getElementById('backLang').value;
    const level = document.getElementById('level').value;

    await fetch('/admin/categories', {
        method: 'POST',
        headers: { 'Content-Type': 'application/json' },
        body: JSON.stringify({
            categoryName: name,
            frontLanguage: frontLang,
            backLanguage: backLang,
            languageLevel: level
        })
    });

    document.getElementById('categoryName').value = '';
    document.getElementById('frontLang').value = '';
    document.getElementById('backLang').value = '';
    document.getElementById('level').value = '';
    loadCategories();
}

async function loadFlashCards(id_Category, categoryName) {
    const res = await fetch(`/admin/flashcards/${id_Category}`);
    const flashcards = await res.json();

    document.getElementById('flashcardsTitle').textContent = `Flashcards for: ${categoryName}`;
    document.getElementById('flashcardsTable').style.display = 'table';
    document.getElementById('flashcardForm').style.display = 'block';

    const tbody = document.getElementById('flashcardsBody');
    tbody.innerHTML = '';

    if (!flashcards.length) {
        const row = document.createElement('tr');
        row.innerHTML = `<td colspan="2">No flashcards found.</td>`;
        tbody.appendChild(row);
    } else {
        flashcards.forEach(f => {
            const row = document.createElement('tr');
            row.innerHTML = `<td>${f.frontFlashCard}</td><td>${f.backFlashCard}</td>`;
            tbody.appendChild(row);
        });
    }

    document.getElementById('addFlashcardBtn').onclick = async () => {
        const question = document.getElementById('flashcardQuestion').value;
        const answer = document.getElementById('flashcardAnswer').value;

        if (!question || !answer) return alert("Fill in both fields!");

        await fetch('/admin/flashcards', {
            method: 'POST',
            headers: { 'Content-Type': 'application/json' },
            body: JSON.stringify({
                ID_Category: id_Category,
                FrontFlashCard: question,
                BackFlashCard: answer
            })
        });

        document.getElementById('flashcardQuestion').value = '';
        document.getElementById('flashcardAnswer').value = '';

        loadFlashCards(id_Category, categoryName);
    };
}

function logout() {
    document.cookie = "admin_logged_in=; expires=Thu, 01 Jan 1970 00:00:00 UTC; path=/admin;";
    window.location.href = "login.html";
}

document.addEventListener('DOMContentLoaded', () => {
    loadUsers();
    loadLogs();
    loadCategories();
    document.getElementById('addCategoryBtn').onclick = addCategory;
});