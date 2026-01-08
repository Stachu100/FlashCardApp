/* -------------------------------------------------------------------------- */
/*                              Sekcja: Users                                 */
/* -------------------------------------------------------------------------- */

async function loadUsers() {
    const res = await fetch('/admin/users');
    const users = await res.json();
    const ul = document.getElementById('users');
    ul.innerHTML = '';

    for (const u of users) {
        const li = document.createElement('li');

        let detailsText = '';
        try {
            const detailsRes = await fetch(`/admin/userdetails/${u.iD_User}`);
            if (detailsRes.ok) {
                const details = await detailsRes.json();
                detailsText = `${details.firstName} ${details.lastName}, ${details.email}, ${details.country}`;
            } else {
                detailsText = 'Details not found';
            }
        } catch (err) {
            detailsText = 'Error loading details';
        }

        const span = document.createElement('span');
        span.textContent = `${u.iD_User} ${u.userName} (${detailsText})`;

        const toggleBtn = document.createElement('button');
        toggleBtn.textContent = u.is_active ? 'Deactivate' : 'Activate';
        toggleBtn.className = `toggle-btn ${u.is_active ? 'active' : 'deactivate'}`;
        toggleBtn.onclick = async () => {
            await toggleUser(u.iD_User);
            await loadUsers();
        };

        li.appendChild(span);
        li.appendChild(toggleBtn);

        ul.appendChild(li);
    }
}

async function toggleUser(id) {
    const res = await fetch(`/admin/users/${id}/toggle`, { method: 'PUT' });
    if (!res.ok) {
        alert('Failed to change user status.');
    }
}

/* -------------------------------------------------------------------------- */
/*                              Sekcja: Logs                                  */
/* -------------------------------------------------------------------------- */

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

/* -------------------------------------------------------------------------- */
/*                              Sekcja: Categories                            */
/* -------------------------------------------------------------------------- */

let editingCategoryId = null;

async function loadCategories() {
    const res = await fetch('/admin/categories');
    const categories = await res.json();
    const ul = document.getElementById('categories');
    ul.innerHTML = '';

    categories.forEach(c => {
        const li = document.createElement('li');

        const span = document.createElement('span');
        span.textContent = `${c.iD_Category} ${c.categoryName} (${c.frontLanguage} → ${c.backLanguage})`;
        span.onclick = () => loadFlashCards(c.iD_Category, c.categoryName);

        const editBtn = document.createElement('button');
        editBtn.textContent = '✎';
        editBtn.className = 'edit-btn';
        editBtn.onclick = () => editCategory(c);

        const delBtn = document.createElement('button');
        delBtn.textContent = '🗑';
        delBtn.className = 'delete-btn';
        delBtn.onclick = () => deleteCategory(c.iD_Category);

        li.appendChild(span);
        li.appendChild(editBtn);
        li.appendChild(delBtn);

        ul.appendChild(li);
    });
}

async function addCategory() {
    const name = document.getElementById('categoryName').value;
    const frontLang = document.getElementById('frontLang').value;
    const backLang = document.getElementById('backLang').value;
    const level = document.getElementById('level').value;

    if (!name || !frontLang || !backLang) return;

    if (editingCategoryId) {
        await fetch(`/admin/categories/${editingCategoryId}`, {
            method: 'PUT',
            headers: { 'Content-Type': 'application/json' },
            body: JSON.stringify({
                categoryName: name,
                frontLanguage: frontLang,
                backLanguage: backLang,
                languageLevel: level
            })
        });
    }
    else {
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
    }

    editingCategoryId = null;
    document.getElementById('categoryName').value = '';
    document.getElementById('frontLang').value = '';
    document.getElementById('backLang').value = '';
    document.getElementById('level').value = 'Brak';
    document.getElementById('addCategoryBtn').textContent = "Add Category";
    document.getElementById('categoryTitle').textContent = "Add Category";

    loadCategories();
}

function editCategory(category) {
    editingCategoryId = category.iD_Category;

    document.getElementById('categoryName').value = category.categoryName;
    document.getElementById('frontLang').value = category.frontLanguage;
    document.getElementById('backLang').value = category.backLanguage;
    document.getElementById('level').value = category.languageLevel || "Brak";

    document.getElementById('addCategoryBtn').textContent = "Save";
    document.getElementById('categoryTitle').textContent = "Edit Category";
}

async function deleteCategory(id) {
    if (!confirm("Delete this category (and its flashcards)?")) return;

    await fetch(`/admin/categories/${id}`, { method: 'DELETE' });
    loadCategories();
}

/* -------------------------------------------------------------------------- */
/*                              Sekcja: FlashCards                            */
/* -------------------------------------------------------------------------- */

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

/* -------------------------------------------------------------------------- */
/*                              Sekcja: Admin Menu                            */
/* -------------------------------------------------------------------------- */

const usernameCookie = document.cookie.split(';').map(c => c.trim()).find(c => c.startsWith('admin_username='));

const username = usernameCookie ? decodeURIComponent(usernameCookie.split('=')[1]) : 'Admin';
document.getElementById('username').textContent = username;

document.addEventListener('DOMContentLoaded', () => {
    const logoutBtn = document.getElementById('logoutBtn');
    if (logoutBtn) logoutBtn.onclick = logout;
});

document.addEventListener('DOMContentLoaded', () => {
    loadUsers();
    loadLogs();
    loadCategories();
    loadNotificationsOnApiring();
    document.getElementById('addCategoryBtn').onclick = addCategory;
});

function logout() {
    document.cookie = "admin_logged_in=; path=/; expires=Thu, 01 Jan 1970 00:00:00 UTC;";
    document.cookie = "admin_username=; path=/; expires=Thu, 01 Jan 1970 00:00:00 UTC;";
    window.location.href = "login.html";
}

/* -------------------------------------------------------------------------- */
/*                              Sekcja: Powiadomienia                         */
/* -------------------------------------------------------------------------- */

const notifBtn = document.getElementById('notificationsBtn');
const dropdown = document.getElementById('notification-dropdown');
const ul = document.getElementById('notifications');
const badge = document.getElementById('badge-count');
const clearBtn = document.getElementById('clear-btn');

async function loadNotificationsOnApiring() {
    try {
        const res = await fetch('/admin/notifications');
        const notifications = await res.json();
        let i = 0;

        notifications.forEach((n) => {
            if (!n.is_read) i++;
        });

        badge.textContent = i;

        } catch (err) { }    
    } 

async function loadNotifications() {
    try {
        const res = await fetch('/admin/notifications');
        const notifications = await res.json();

        ul.innerHTML = '';

        //Renderuj powiadomienia
        notifications.forEach((n) => {
            const li = document.createElement('li');
            li.classList.add('notification-item');
            if (!n.is_read) li.classList.add('notification-Unread');
            

            li.innerHTML = `
            <div class="notif-row">
              <div class="notif-text">
                <strong>${n.tableName}</strong> <small>Action:  ${n.action}<small> ${!n.is_read ? `<strong>New!!!</strong>`: ``} <br>
                <a> User: ${n.userName} ${n.tableName === `User` ? n.user_Is_active ? `,` + 'Deactivate' : `,` + 'Activate' : ``}</a> <br>
                <a> ${n.tableName === `Category` ? `CategoryName: ` + n.categoryName + `<br>`  : ``} </a>
                <a>Time: ${ new Date(n.actionDate).toLocaleString() }</a><br>
              </div>
            </div>
          `;

            ul.appendChild(li);
        });
        const readNot = await fetch(`/admin/Readnotifications`, { method: 'PUT' });
        if (!readNot.ok) {
            alert('Failed to read notifications.');
        }
    } catch (err) {
        ul.innerHTML = `<li>Error</li>`;
    }
}

//Otwórz/zamknij pop-uop
notifBtn.addEventListener('click', async () => {
    const isVisible = dropdown.style.display === 'block';
    dropdown.style.display = isVisible ? 'none' : 'block';
    if (!isVisible) await loadNotifications();
});

//Zamknij pop-up klikając poza nim
document.addEventListener('click', (event) => {
    if (!notifBtn.contains(event.target) && !dropdown.contains(event.target)) {
        const isVisible = dropdown.style.display === 'block';
        dropdown.style.display = 'none';
        
        if (isVisible) badge.textContent = 0;
    }
});