
const API_BASE = "http://localhost:5165/api";

async function handleResponse(response) {
    let data;

    try {
        data = await response.json();
    } catch {
        data = { message: "Unexpected server error" };
    }

    if (!response.ok) {
        throw data;
    }

    return data;
}

function showAlert(message, type = "success", containerId = "alert-container") {
    const container = document.getElementById(containerId);

    if (!container) return;

    container.innerHTML = `
    < div class="alert alert-${type}" >
        ${ message }
        </div >
    `;

    setTimeout(() => {
        container.innerHTML = "";
    }, 5000);
}

function getStatusBadge(status) {
    const map = {
        "Todo": "badge-todo",
        "In Progress": "badge-inprogress",
        "Done": "badge-done"
    };

    return `
    < span class="badge ${map[status]}" >
        ${ status }
        </span >
    `;
}

async function loadUsersForDropdown() {

    const response =
        await fetch(`${API_BASE}/users`);

    const result =
        await response.json();

    const select =
        document.getElementById("userId");

    select.innerHTML =
        '<option value="">Select User</option>';

    result.data.forEach(user => {

        select.innerHTML += `
            <option value="${user.userId}">
                ${user.userName}
            </option>
        `;
    });
}



document.addEventListener("DOMContentLoaded", () => {
    loadUsersForDropdown("userId");
});

const form = document.getElementById("add-task-form");

if (form) {

    form.addEventListener("submit", async e => {

        e.preventDefault();

        const task = {
            title:
                document.getElementById("title").value.trim(),

            description:
                document.getElementById("description").value.trim(),

            status:
                document.getElementById("status").value,

            userId:
                Number(document.getElementById("userId").value)
        };

        if (!task.title) {
            return showAlert("Title required", "danger");
        }

        try {

            const response = await fetch(`${API_BASE}/tasks`, {
                method: "POST",
                headers: {
                    "Content-Type": "application/json"
                },
                body: JSON.stringify(task)
            });

            const result = await handleResponse(response);

            if (result.success) {

                showAlert("Task created successfully");

                form.reset();

                setTimeout(() => {
                    location.href = "tasks.html";
                }, 1000);
            }

        } catch (error) {

            showAlert(
                error.message || "Create failed",
                "danger"
            );
        }

    });

}

async function loadUsersTable() {

    try {

        const response =
            await fetch(`${API_BASE}/users`);

        const result =
            await handleResponse(response);

        const tbody =
            document.getElementById("user-list");

        if (!tbody) return;

        if (!result.data.length) {

            tbody.innerHTML =
                `<tr>
                    <td colspan="4">
                        No users found
                    </td>
                </tr>`;

            return;
        }

        tbody.innerHTML =
            result.data.map(user => `

            <tr>

                <td>${user.userId}</td>

                <td>${user.userName}</td>

                <td>${user.email}</td>

                <td>

                    <button
                        class="btn btn-primary"

                        onclick="location.href='user-detail.html?id=${user.userId}'">

                        View Tasks

                    </button>

                </td>

            </tr>

        `).join("");

    } catch {

        showAlert(
            "Unable to load users",
            "danger"
        );
    }

}

document.addEventListener(
    "DOMContentLoaded",
    loadUsersTable
);

async function loadDashboard() {

    try {

        const response =
            await fetch(`${API_BASE}/tasks`);

        const result =
            await handleResponse(response);

        const tasks =
            result.data || [];

        document.getElementById("stat-total").textContent =
            tasks.length;

        document.getElementById("stat-todo").textContent =
            tasks.filter(x => x.status === "Todo").length;

        document.getElementById("stat-inprogress").textContent =
            tasks.filter(x => x.status === "In Progress").length;

        document.getElementById("stat-done").textContent =
            tasks.filter(x => x.status === "Done").length;

        const tbody =
            document.getElementById("recent-tasks");

        tbody.innerHTML =
            tasks.slice(0, 5).map(task => `

            <tr>

                <td>${task.title}</td>

                <td>${getStatusBadge(task.status)}</td>

                <td>${task.userName}</td>

                <td>${formatDate(task.createdDate)}</td>

            </tr>

        `).join("");

    } catch {

        showAlert(
            "Dashboard load failed",
            "danger"
        );
    }

}

document.addEventListener(
    "DOMContentLoaded",
    loadDashboard
);

