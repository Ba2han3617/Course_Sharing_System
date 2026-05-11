/* =====================================================================
   site.js – CourseNotes JavaScript
   Handles: dark mode, live search, AJAX interactions, toast, drag-drop
   ===================================================================== */

// ─── Dark Mode ─────────────────────────────────────────────────────────────
(function () {
    'use strict';

    const DARK_KEY = 'cnss-dark-mode';
    const html = document.documentElement;
    const icon = document.getElementById('darkIcon');

    function applyTheme(isDark) {
        html.setAttribute('data-bs-theme', isDark ? 'dark' : 'light');
        if (icon) {
            icon.className = isDark ? 'bi bi-sun-fill' : 'bi bi-moon-fill';
        }
    }

    // Init from storage
    const saved = localStorage.getItem(DARK_KEY);
    const prefersDark = window.matchMedia('(prefers-color-scheme: dark)').matches;
    applyTheme(saved !== null ? saved === 'true' : prefersDark);

    // Toggle button
    document.addEventListener('DOMContentLoaded', () => {
        const btn = document.getElementById('darkModeToggle');
        if (btn) {
            btn.addEventListener('click', () => {
                const isDark = html.getAttribute('data-bs-theme') === 'dark';
                applyTheme(!isDark);
                localStorage.setItem(DARK_KEY, String(!isDark));
            });
        }
    });
})();

// ─── Live Global Search ─────────────────────────────────────────────────────
document.addEventListener('DOMContentLoaded', () => {
    const input = document.getElementById('globalSearch');
    const dropdown = document.getElementById('searchDropdown');
    if (!input || !dropdown) return;

    let debounceTimer;

    input.addEventListener('input', () => {
        clearTimeout(debounceTimer);
        const q = input.value.trim();

        if (!q || q.length < 2) {
            dropdown.classList.add('d-none');
            dropdown.innerHTML = '';
            return;
        }

        debounceTimer = setTimeout(async () => {
            try {
                const res = await fetch(`/Notes/Search?q=${encodeURIComponent(q)}`);
                const data = await res.json();

                if (!data.length) {
                    dropdown.innerHTML = '<div class="p-3 text-muted small text-center">No results found</div>';
                    dropdown.classList.remove('d-none');
                    return;
                }

                dropdown.innerHTML = data.map(n => `
                    <a href="/Notes/Detail/${n.id}" class="search-item">
                        <i class="bi bi-file-earmark text-primary"></i>
                        <div class="overflow-hidden">
                            <div class="fw-semibold text-truncate">${escHtml(n.title)}</div>
                            <div class="text-muted small">${escHtml(n.courseName)} &bull; ${escHtml(n.uploaderName)}</div>
                        </div>
                        <span class="badge ms-auto" style="background:rgba(99,102,241,.1);color:var(--primary)">${n.fileType.toUpperCase()}</span>
                    </a>
                `).join('');
                dropdown.classList.remove('d-none');
            } catch (e) {
                console.error('Search error', e);
            }
        }, 280);
    });

    // Close dropdown when clicking outside
    document.addEventListener('click', e => {
        if (!input.contains(e.target) && !dropdown.contains(e.target)) {
            dropdown.classList.add('d-none');
        }
    });
});

// ─── Like Button (AJAX) ─────────────────────────────────────────────────────
function setupLikeButton() {
    const btn = document.getElementById('likeBtn');
    if (!btn) return;

    btn.addEventListener('click', async () => {
        const noteId = parseInt(btn.dataset.noteId);
        try {
            const res = await postJson('/Interaction/ToggleLike', noteId);
            const data = await res.json();
            if (res.status === 401) { showToast('Please login to like notes.', 'warning'); return; }
            btn.classList.toggle('liked', data.isLiked);
            btn.querySelector('.like-count').textContent = data.count;
            const icon = btn.querySelector('i');
            icon.className = data.isLiked ? 'bi bi-heart-fill' : 'bi bi-heart';
        } catch (e) { showToast('Error toggling like.', 'danger'); }
    });
}

// ─── Favorite Button (AJAX) ─────────────────────────────────────────────────
function setupFavoriteButton() {
    const btn = document.getElementById('favoriteBtn');
    if (!btn) return;

    btn.addEventListener('click', async () => {
        const noteId = parseInt(btn.dataset.noteId);
        try {
            const res = await postJson('/Interaction/ToggleFavorite', noteId);
            if (res.status === 401) { showToast('Please login to save favorites.', 'warning'); return; }
            const data = await res.json();
            btn.classList.toggle('favorited', data.isFavorited);
            const icon = btn.querySelector('i');
            icon.className = data.isFavorited ? 'bi bi-bookmark-fill' : 'bi bi-bookmark';
            showToast(data.isFavorited ? 'Added to favorites!' : 'Removed from favorites.', 'success');
        } catch (e) { showToast('Error updating favorites.', 'danger'); }
    });
}

// ─── Rating Stars (Interactive) ─────────────────────────────────────────────
function setupRatingStars() {
    const container = document.querySelector('.rating-stars[data-note-id]');
    if (!container) return;

    const noteId = parseInt(container.dataset.noteId);
    const stars = container.querySelectorAll('.star');

    stars.forEach((star, i) => {
        star.addEventListener('mouseenter', () => highlightStars(stars, i + 1));
        star.addEventListener('mouseleave', () => {
            const current = parseInt(container.dataset.current || 0);
            highlightStars(stars, current);
        });
        star.addEventListener('click', async () => {
            const score = i + 1;
            try {
                const res = await postJson('/Interaction/Rate', { noteId, score });
                if (res.status === 401) { showToast('Please login to rate.', 'warning'); return; }
                const data = await res.json();
                container.dataset.current = score;
                highlightStars(stars, score);
                document.getElementById('avgRating').textContent = data.avg.toFixed(1);
                document.getElementById('ratingCount').textContent = data.count;
                showToast('Rating saved!', 'success');
            } catch (e) { showToast('Error saving rating.', 'danger'); }
        });
    });
}

function highlightStars(stars, upTo) {
    stars.forEach((s, i) => s.classList.toggle('active', i < upTo));
}

// ─── Comment System ─────────────────────────────────────────────────────────
function setupCommentSystem() {
    const form = document.getElementById('commentForm');
    if (!form) return;

    form.addEventListener('submit', async e => {
        e.preventDefault();
        const noteId = parseInt(form.dataset.noteId);
        const input = form.querySelector('textarea[name="content"]');
        const content = input.value.trim();
        if (!content) return;

        try {
            const res = await postJson('/Interaction/AddComment', { noteId, content });
            if (res.status === 401) { showToast('Please login to comment.', 'warning'); return; }
            if (!res.ok) { showToast('Error posting comment.', 'danger'); return; }

            const data = await res.json();
            const list = document.getElementById('commentList');
            const el = document.createElement('div');
            el.className = 'comment-card fade-in-up';
            el.innerHTML = `
                <div class="d-flex align-items-start gap-3">
                    <div class="avatar-sm rounded-circle bg-primary d-flex align-items-center justify-content-center text-white fw-bold flex-shrink-0">
                        ${escHtml(data.userName[0])}
                    </div>
                    <div class="flex-grow-1">
                        <div class="d-flex justify-content-between align-items-start">
                            <strong>${escHtml(data.userName)}</strong>
                            <small class="text-muted">${data.createdAt}</small>
                        </div>
                        <p class="mb-0 mt-1">${escHtml(data.content)}</p>
                    </div>
                </div>
            `;
            list.prepend(el);
            input.value = '';
            showToast('Comment posted!', 'success');
        } catch (e) { showToast('Error posting comment.', 'danger'); }
    });

    // Delete comment buttons
    document.addEventListener('click', async e => {
        const btn = e.target.closest('.delete-comment-btn');
        if (!btn) return;
        if (!confirm('Delete this comment?')) return;
        const commentId = parseInt(btn.dataset.commentId);

        try {
            const res = await postJson('/Interaction/DeleteComment', commentId);
            if (res.ok) {
                btn.closest('.comment-card').remove();
                showToast('Comment deleted.', 'success');
            }
        } catch (e) { showToast('Error deleting comment.', 'danger'); }
    });
}

// ─── File Upload Preview ─────────────────────────────────────────────────────
function setupUploadZone() {
    const zone = document.getElementById('uploadZone');
    const input = document.getElementById('fileInput');
    const preview = document.getElementById('filePreview');
    if (!zone || !input) return;

    zone.addEventListener('click', () => input.click());

    ['dragenter', 'dragover'].forEach(e => {
        zone.addEventListener(e, ev => { ev.preventDefault(); zone.classList.add('dragging'); });
    });
    ['dragleave', 'drop'].forEach(e => {
        zone.addEventListener(e, ev => { ev.preventDefault(); zone.classList.remove('dragging'); });
    });
    zone.addEventListener('drop', e => {
        if (e.dataTransfer.files[0]) {
            input.files = e.dataTransfer.files;
            showFilePreview(e.dataTransfer.files[0]);
        }
    });

    input.addEventListener('change', () => {
        if (input.files[0]) showFilePreview(input.files[0]);
    });

    function showFilePreview(file) {
        if (!preview) return;
        const sizeMB = (file.size / 1024 / 1024).toFixed(2);
        preview.innerHTML = `
            <div class="d-flex align-items-center gap-3 p-3 bg-success bg-opacity-10 border border-success rounded-3">
                <i class="bi bi-file-check fs-2 text-success"></i>
                <div>
                    <div class="fw-semibold">${escHtml(file.name)}</div>
                    <div class="text-muted small">${sizeMB} MB</div>
                </div>
                <i class="bi bi-check-circle-fill text-success ms-auto fs-4"></i>
            </div>
        `;
    }
}

// ─── Helpers ────────────────────────────────────────────────────────────────
async function postJson(url, body) {
    const token = document.querySelector('input[name="__RequestVerificationToken"]')?.value ?? '';
    return fetch(url, {
        method: 'POST',
        headers: {
            'Content-Type': 'application/json',
            'RequestVerificationToken': token
        },
        body: JSON.stringify(body)
    });
}

function escHtml(str) {
    const d = document.createElement('div');
    d.textContent = str;
    return d.innerHTML;
}

function showToast(message, type = 'success') {
    const container = document.getElementById('toastContainer');
    if (!container) return;

    const colorMap = { success: 'text-bg-success', danger: 'text-bg-danger', warning: 'text-bg-warning', info: 'text-bg-info' };
    const bgClass = colorMap[type] || 'text-bg-secondary';

    const el = document.createElement('div');
    el.className = `toast align-items-center ${bgClass} border-0`;
    el.setAttribute('role', 'alert');
    el.innerHTML = `
        <div class="d-flex">
            <div class="toast-body">${escHtml(message)}</div>
            <button type="button" class="btn-close btn-close-white me-2 m-auto" data-bs-dismiss="toast"></button>
        </div>
    `;
    container.appendChild(el);
    const t = new bootstrap.Toast(el, { delay: 3500 });
    t.show();
    el.addEventListener('hidden.bs.toast', () => el.remove());
}

// ─── Init ────────────────────────────────────────────────────────────────────
document.addEventListener('DOMContentLoaded', () => {
    setupLikeButton();
    setupFavoriteButton();
    setupRatingStars();
    setupCommentSystem();
    setupUploadZone();

    // Confirm delete forms
    document.querySelectorAll('form[data-confirm]').forEach(f => {
        f.addEventListener('submit', e => {
            if (!confirm(f.dataset.confirm)) e.preventDefault();
        });
    });
});
