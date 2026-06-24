// API Base URL
const API_BASE = 'https://localhost:7000/api'; // Update with your actual port

// Utility function to handle API responses
async function handleResponse(response) {
    const data = await response.json();
    if (!response.ok) {
        throw data;
    }
    return data;
}

// Display alert messages
function showAlert(message, type = 'success', containerId = 'alert-container') {
    const container = document.getElementById(containerId);
    if (!container) return;

    const alert = document.createElement('div');
    alert.className = `alert alert-${type} show`;
    alert.textContent = message;

    // Remove existing alerts
    const existingAlerts = container.querySelectorAll('.alert');
    existingAlerts.forEach(el => el.remove());

    container.appendChild(alert);

    // Auto-hide after 5 seconds
    setTimeout(() => {
        alert.classList.remove('show');
        setTimeout(() => alert.remove(), 300);
    }, 5000);
}

// Load all users for dropdown
async function loadUsersForDropdown(dropdownId) {
    try {
        const response = await fetch(`${API_BASE}/users`);
        const result = await handleResponse(response);

        const dropdown = document.getElementById(dropdownId);
        if (!dropdown) return;

        dropdown.innerHTML = '<option value="">Select a user</option>';
        result.data.forEach(user => {
            const option = document.createElement('option');
            option.value = user.userId;
            option.textContent = `${user.userName} (${user.email})`;
            dropdown.appendChild(option);
        });
    } catch (error) {
        console.error('Error loading users:', error);
    }
}

// Get status badge HTML
function getStatusBadge(status) {
    const statusMap = {
        'Todo': 'badge-todo',
        'In Progress': 'badge-inprogress',
        'Done': 'badge-done'
    };
    const badgeClass = statusMap[status] || 'badge-todo';
    return `<span class="badge ${badgeClass}">${status}</span>`;
}

// Format date
function formatDate(dateString) {
    const date = new Date(dateString);
    return date.toLocaleDateString('en-US', {
        year: 'numeric',
        month: 'short',
        day: 'numeric',
        hour: '2-digit',
        minute: '2-digit'
    });
}