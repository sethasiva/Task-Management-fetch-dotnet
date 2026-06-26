// ============================================================
// TASK MANAGEMENT SYSTEM — app.js
// Backend: ASP.NET Core Web API (Controller-based)
// API Base URL — change this to match your local API port
// ============================================================

const API_BASE = 'http://localhost:5165/api';

// ── Utility: fetch wrapper ─────────────────────────────────
async function apiFetch(path, options = {}) {
  const url = `${API_BASE}${path}`;
  const defaults = {
    headers: { 'Content-Type': 'application/json' }
  };
  const config = { ...defaults, ...options };
  if (options.body) config.body = options.body;

  const res = await fetch(url, config);
  const data = await res.json();
  return { status: res.status, data };
}

// ── Status helpers ─────────────────────────────────────────
function statusBadge(status) {
  const map = {
    'Todo':        'badge-todo',
    'In Progress': 'badge-inprog',
    'Done':        'badge-done',
  };
  const cls = map[status] || 'badge-todo';
  return `<span class="badge ${cls}">${status}</span>`;
}

// ── Alert helpers ──────────────────────────────────────────
function showAlert(containerId, type, message, errors = []) {
  const el = document.getElementById(containerId);
  if (!el) return;
  const icon = type === 'success' ? '✓' : '✕';
  let html = `<div class="alert alert-${type}"><span>${icon}</span><div><strong>${message}</strong>`;
  if (errors.length) {
    html += `<ul>${errors.map(e => `<li>${e}</li>`).join('')}</ul>`;
  }
  html += `</div></div>`;
  el.innerHTML = html;
  el.scrollIntoView({ behavior: 'smooth', block: 'nearest' });
}

function clearAlert(containerId) {
  const el = document.getElementById(containerId);
  if (el) el.innerHTML = '';
}

// ── Button loading state ───────────────────────────────────
function setLoading(btn, loading, text = 'Save') {
  if (!btn) return;
  btn.disabled = loading;
  btn.innerHTML = loading
    ? `<span class="spinner"></span> Processing…`
    : text;
}

// ── Active nav link ────────────────────────────────────────
function setActiveNav() {
  const page = location.pathname.split('/').pop();
  document.querySelectorAll('.nav-link').forEach(link => {
    const href = link.getAttribute('href').split('/').pop();
    if (href === page) link.classList.add('active');
  });
}

// ── Escape HTML to prevent XSS ────────────────────────────
function escHtml(str) {
  if (!str) return '';
  return String(str)
    .replace(/&/g, '&amp;')
    .replace(/</g, '&lt;')
    .replace(/>/g, '&gt;')
    .replace(/"/g, '&quot;');
}

// ══════════════════════════════════════════════════════════
// USER API FUNCTIONS
// ══════════════════════════════════════════════════════════

// GET /api/users
async function loadUsers() {
  const tbody = document.getElementById('users-tbody');
  if (!tbody) return;
  tbody.innerHTML = `<tr><td colspan="4"><div class="loading-overlay"><div class="loading-spinner-dark"></div></div></td></tr>`;
  try {
    const { data } = await apiFetch('/users');
    if (data.success && data.data.length > 0) {
      tbody.innerHTML = data.data.map(u => `
        <tr>
          <td>${u.userId}</td>
          <td><strong>${escHtml(u.userName)}</strong></td>
          <td>${escHtml(u.email)}</td>
          <td class="actions">
            <a href="user-detail.html?id=${u.userId}" class="btn btn-outline btn-sm">👁 View Tasks</a>
          </td>
        </tr>`).join('');
    } else {
      tbody.innerHTML = `<tr><td colspan="4"><div class="empty-state">
        <div class="empty-icon">👤</div>
        <h3>No users yet</h3>
        <p>Add your first user using the form above.</p>
      </div></td></tr>`;
    }
  } catch {
    tbody.innerHTML = `<tr><td colspan="4"><div class="alert alert-error">Failed to load users. Is the API running?</div></td></tr>`;
  }
}

// POST /api/users
async function addUser() {
  const btn = document.getElementById('btn-add-user');
  const userName = document.getElementById('userName')?.value?.trim();
  const email    = document.getElementById('email')?.value?.trim();
  clearAlert('user-alert');

  // Frontend validation
  const errors = [];
  if (!userName) errors.push('UserName is required.');
  else if (userName.length > 100) errors.push('UserName must be under 100 characters.');
  if (!email) errors.push('Email is required.');
  else if (!/^[^\s@]+@[^\s@]+\.[^\s@]+$/.test(email)) errors.push('Enter a valid email address.');
  else if (email.length > 100) errors.push('Email must be under 100 characters.');
  if (errors.length) { showAlert('user-alert', 'error', 'Please fix the following errors:', errors); return; }

  setLoading(btn, true);
  try {
    const { status, data } = await apiFetch('/users', {
      method: 'POST',
      body: JSON.stringify({ userName, email })
    });
    if (data.success) {
      showAlert('user-alert', 'success', 'User created successfully!');
      document.getElementById('userName').value = '';
      document.getElementById('email').value = '';
      loadUsers();
    } else {
      showAlert('user-alert', 'error', data.message || 'Failed to add user.', data.errors || []);
    }
  } catch {
    showAlert('user-alert', 'error', 'Network error. Check if the API is running.');
  } finally {
    setLoading(btn, false, '+ Add User');
  }
}

// GET /api/users/{id}/tasks  (User Detail page)
async function loadUserWithTasks() {
  const params = new URLSearchParams(location.search);
  const id = params.get('id');
  if (!id) { window.location.href = 'users.html'; return; }

  const detailEl = document.getElementById('user-detail');
  const tasksEl  = document.getElementById('user-tasks-tbody');

  try {
    const { status, data } = await apiFetch(`/users/${id}/tasks`);
    if (status === 404 || !data.success) {
      detailEl.innerHTML = `<div class="alert alert-error">${data.message || 'User not found.'}</div>`;
      return;
    }
    const u = data.data;
    const initials = u.userName.split(' ').map(w => w[0]).join('').toUpperCase().slice(0, 2);
    detailEl.innerHTML = `
      <div class="user-info-card">
        <div class="user-avatar">${initials}</div>
        <div class="user-info">
          <h2>${escHtml(u.userName)}</h2>
          <p>${escHtml(u.email)}</p>
          <p style="margin-top:6px;font-size:.82rem;color:var(--color-primary);font-weight:600;">
            ${u.tasks.length} task(s) assigned
          </p>
        </div>
      </div>`;

    if (u.tasks.length === 0) {
      tasksEl.innerHTML = `<tr><td colspan="4"><div class="empty-state">
        <div class="empty-icon">📋</div>
        <h3>No tasks assigned</h3>
        <p>This user has no tasks yet.</p>
      </div></td></tr>`;
    } else {
      tasksEl.innerHTML = u.tasks.map(t => `
        <tr>
          <td><strong>${escHtml(t.title)}</strong></td>
          <td>${escHtml(t.description || '—')}</td>
          <td>${statusBadge(t.status)}</td>
          <td>${new Date(t.createdDate).toLocaleDateString()}</td>
        </tr>`).join('');
    }
  } catch {
    detailEl.innerHTML = `<div class="alert alert-error">Network error. Is the API running?</div>`;
  }
}

// ══════════════════════════════════════════════════════════
// TASK API FUNCTIONS
// ══════════════════════════════════════════════════════════

// GET /api/tasks  (also used by dashboard)
async function loadTasks(keyword = '') {
  const tbody = document.getElementById('tasks-tbody');
  if (!tbody) return;
  tbody.innerHTML = `<tr><td colspan="7"><div class="loading-overlay"><div class="loading-spinner-dark"></div></div></td></tr>`;

  const endpoint = keyword
    ? `/tasks/search?name=${encodeURIComponent(keyword)}`
    : '/tasks';
  try {
    const { data } = await apiFetch(endpoint);
    if (data.success && data.data.length > 0) {
      tbody.innerHTML = data.data.map(t => `
        <tr>
          <td>${t.taskId}</td>
          <td><strong>${escHtml(t.title)}</strong><br><small style="color:var(--color-muted)">${escHtml(t.description || '')}</small></td>
          <td>${escHtml(t.userName)}</td>
          <td>
            <select class="status-select" onchange="changeStatus(${t.taskId}, this.value, this)">
              <option ${t.status==='Todo'?'selected':''}>Todo</option>
              <option ${t.status==='In Progress'?'selected':''}>In Progress</option>
              <option ${t.status==='Done'?'selected':''}>Done</option>
            </select>
          </td>
          <td>${new Date(t.createdDate).toLocaleDateString()}</td>
          <td class="actions">
            <a href="edit-task.html?id=${t.taskId}" class="btn btn-outline btn-sm">✏ Edit</a>
            <button class="btn btn-danger btn-sm" onclick="deleteTask(${t.taskId})">🗑 Delete</button>
          </td>
        </tr>`).join('');
    } else {
      tbody.innerHTML = `<tr><td colspan="7"><div class="empty-state">
        <div class="empty-icon">📋</div>
        <h3>${keyword ? 'No tasks found' : 'No tasks yet'}</h3>
        <p>${keyword ? 'Try a different search keyword.' : 'Add your first task to get started.'}</p>
      </div></td></tr>`;
    }
  } catch {
    tbody.innerHTML = `<tr><td colspan="7"><div class="alert alert-error">Failed to load tasks. Is the API running?</div></td></tr>`;
  }
}

// GET /api/tasks  → dashboard counts
async function loadDashboard() {
  try {
    const { data } = await apiFetch('/tasks');
    const tasks = data.success ? data.data : [];
    const total   = tasks.length;
    const todo    = tasks.filter(t => t.status === 'Todo').length;
    const inprog  = tasks.filter(t => t.status === 'In Progress').length;
    const done    = tasks.filter(t => t.status === 'Done').length;

    setText('count-total',  total);
    setText('count-todo',   todo);
    setText('count-inprog', inprog);
    setText('count-done',   done);

    // Render recent tasks on dashboard
    const recentEl = document.getElementById('recent-tasks-tbody');
    if (recentEl) {
      const recent = [...tasks].slice(0, 8);
      if (recent.length) {
        recentEl.innerHTML = recent.map(t => `
          <tr>
            <td><strong>${escHtml(t.title)}</strong></td>
            <td>${escHtml(t.userName)}</td>
            <td>${statusBadge(t.status)}</td>
            <td>${new Date(t.createdDate).toLocaleDateString()}</td>
          </tr>`).join('');
      } else {
        recentEl.innerHTML = `<tr><td colspan="4"><div class="empty-state"><div class="empty-icon">📋</div><h3>No tasks yet</h3></div></td></tr>`;
      }
    }
  } catch {
    ['count-total','count-todo','count-inprog','count-done'].forEach(id => setText(id, '—'));
  }
}

function setText(id, val) {
  const el = document.getElementById(id);
  if (el) el.textContent = val;
}

// POST /api/tasks  (Add Task page)
async function addTask() {
  const btn  = document.getElementById('btn-add-task');
  const title = document.getElementById('task-title')?.value?.trim();
  const desc  = document.getElementById('task-desc')?.value?.trim();
  const status = document.getElementById('task-status')?.value;
  const userId = document.getElementById('task-user')?.value;
  clearAlert('task-alert');

  const errors = [];
  if (!title) errors.push('Title is required.');
  else if (title.length > 200) errors.push('Title must be under 200 characters.');
  if (desc && desc.length > 500) errors.push('Description must be under 500 characters.');
  if (!status) errors.push('Status is required.');
  if (!userId) errors.push('Please select a user.');
  if (errors.length) { showAlert('task-alert', 'error', 'Please fix the following errors:', errors); return; }

  setLoading(btn, true);
  try {
    const { status: httpStatus, data } = await apiFetch('/tasks', {
      method: 'POST',
      body: JSON.stringify({ title, description: desc, status, userId: parseInt(userId) })
    });
    if (data.success) {
      showAlert('task-alert', 'success', 'Task created successfully!');
      setTimeout(() => { window.location.href = 'tasks.html'; }, 1000);
    } else {
      showAlert('task-alert', 'error', data.message || 'Failed to create task.', data.errors || []);
    }
  } catch {
    showAlert('task-alert', 'error', 'Network error. Check if the API is running.');
  } finally {
    setLoading(btn, false, '+ Create Task');
  }
}

// GET /api/tasks/{id}  (Edit Task page – populate form)
async function loadTaskForEdit() {
  const params = new URLSearchParams(location.search);
  const id = params.get('id');
  if (!id) { window.location.href = 'tasks.html'; return; }

  const alertEl = document.getElementById('task-alert');
  try {
    const { status, data } = await apiFetch(`/tasks/${id}`);
    if (status === 404 || !data.success) {
      showAlert('task-alert', 'error', data.message || 'Task not found.');
      return;
    }
    const t = data.data;
    document.getElementById('task-title').value  = t.title;
    document.getElementById('task-desc').value   = t.description || '';
    document.getElementById('task-status').value = t.status;
    // userId will be set after dropdown loads
    window._editTaskUserId = t.userId;
    window._editTaskId = id;
  } catch {
    showAlert('task-alert', 'error', 'Failed to load task.');
  }
}

// PUT /api/tasks/{id}  (Edit Task page)
async function updateTask() {
  const btn    = document.getElementById('btn-update-task');
  const id     = window._editTaskId;
  const title  = document.getElementById('task-title')?.value?.trim();
  const desc   = document.getElementById('task-desc')?.value?.trim();
  const status = document.getElementById('task-status')?.value;
  const userId = document.getElementById('task-user')?.value;
  clearAlert('task-alert');

  const errors = [];
  if (!title) errors.push('Title is required.');
  else if (title.length > 200) errors.push('Title must be under 200 characters.');
  if (desc && desc.length > 500) errors.push('Description must be under 500 characters.');
  if (!status) errors.push('Status is required.');
  if (!userId) errors.push('Please select a user.');
  if (errors.length) { showAlert('task-alert', 'error', 'Please fix the following errors:', errors); return; }

  setLoading(btn, true);
  try {
    const { data } = await apiFetch(`/tasks/${id}`, {
      method: 'PUT',
      body: JSON.stringify({ title, description: desc, status, userId: parseInt(userId) })
    });
    if (data.success) {
      showAlert('task-alert', 'success', 'Task updated successfully!');
      setTimeout(() => { window.location.href = 'tasks.html'; }, 1000);
    } else {
      showAlert('task-alert', 'error', data.message || 'Failed to update task.', data.errors || []);
    }
  } catch {
    showAlert('task-alert', 'error', 'Network error.');
  } finally {
    setLoading(btn, false, '✓ Save Changes');
  }
}

// PUT /api/tasks/{id}/status  (inline in task list)
async function changeStatus(taskId, newStatus, selectEl) {
  try {
    const { data } = await apiFetch(`/tasks/${taskId}/status`, {
      method: 'PUT',
      body: JSON.stringify({ status: newStatus })
    });
    if (!data.success) {
      alert('Failed to update status: ' + data.message);
      loadTasks(); // reload to reset dropdown
    }
  } catch {
    alert('Network error while updating status.');
    loadTasks();
  }
}

// DELETE /api/tasks/{id}
async function deleteTask(taskId) {
  if (!confirm('Are you sure you want to delete this task? This action cannot be undone.')) return;
  try {
    const { status, data } = await apiFetch(`/tasks/${taskId}`, { method: 'DELETE' });
    if (data.success) {
      loadTasks(); // refresh list
    } else {
      alert('Failed to delete task: ' + data.message);
    }
  } catch {
    alert('Network error while deleting task.');
  }
}

// ── Populate user dropdown (Add / Edit task) ───────────────
async function populateUserDropdown(selectedUserId = null) {
  const select = document.getElementById('task-user');
  if (!select) return;
  try {
    const { data } = await apiFetch('/users');
    if (data.success) {
      select.innerHTML = `<option value="">— Select User —</option>` +
        data.data.map(u =>
          `<option value="${u.userId}" ${String(u.userId) === String(selectedUserId) ? 'selected' : ''}>${escHtml(u.userName)}</option>`
        ).join('');
    }
  } catch {
    select.innerHTML = `<option value="">Failed to load users</option>`;
  }
}

// ── Search (Task List page) ────────────────────────────────
function setupSearch() {
  const input = document.getElementById('search-input');
  const btn   = document.getElementById('search-btn');
  const clearBtn = document.getElementById('clear-search');
  if (!input) return;
  btn?.addEventListener('click', () => {
    loadTasks(input.value.trim());
  });
  input.addEventListener('keyup', e => {
    if (e.key === 'Enter') loadTasks(input.value.trim());
    if (!input.value.trim()) loadTasks('');
  });
  clearBtn?.addEventListener('click', () => {
    input.value = '';
    loadTasks('');
  });
}

// ── Init per page ──────────────────────────────────────────
document.addEventListener('DOMContentLoaded', () => {
  setActiveNav();
  const page = location.pathname.split('/').pop();

  if (page === 'dashboard.html' || page === '' || page === 'index.html') {
    loadDashboard();
  }
  if (page === 'users.html') {
    loadUsers();
  }
  if (page === 'user-detail.html') {
    loadUserWithTasks();
  }
  if (page === 'tasks.html') {
    loadTasks();
    setupSearch();
  }
  if (page === 'add-task.html') {
    populateUserDropdown();
  }
  if (page === 'edit-task.html') {
    loadTaskForEdit().then(() => {
      populateUserDropdown(window._editTaskUserId);
    });
  }
});
