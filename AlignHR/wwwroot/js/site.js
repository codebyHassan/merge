// ═══ SIDEBAR TOGGLE ═══
function toggleSidebar() {
    const s = document.getElementById('sidebar'),
        m = document.getElementById('mainWrapper'),
        o = document.getElementById('mobileOverlay');
    if (window.innerWidth <= 768) {
        s.classList.toggle('mobile-open');
        o.classList.toggle('active');
    } else {
        s.classList.toggle('collapsed');
        m.classList.toggle('collapsed');
    }
}

function closeMobileSidebar() {
    document.getElementById('sidebar').classList.remove('mobile-open');
    document.getElementById('mobileOverlay').classList.remove('active');
}

// ═══ SIDEBAR NAV GROUP TOGGLE ═══
function toggleNavGroup(btn) {
    const group = btn.closest('.sb-nav-group');
    if (!group) return;
    
    // Close other open groups at the same level (accordion behavior)
    const parent = group.parentElement;
    parent.querySelectorAll(':scope > .sb-nav-group.open').forEach(g => {
        if (g !== group) g.classList.remove('open');
    });
    
    group.classList.toggle('open');
}

function toggleNestedGroup(btn) {
    const group = btn.closest('.sb-nav-nested-group');
    if (!group) return;
    group.classList.toggle('open');
}

// ═══ THEME TOGGLE ═══
function toggleTheme() {
    const h = document.documentElement, i = document.getElementById('themeIcon');
    if (h.getAttribute('data-theme') === 'light') {
        h.setAttribute('data-theme', 'dark');
        i.innerHTML = '<path d="M21 12.79A9 9 0 1 1 11.21 3 7 7 0 0 0 21 12.79z" fill="none" stroke="currentColor" stroke-width="2"/>';
    } else {
        h.setAttribute('data-theme', 'light');
        i.innerHTML = '<circle cx="12" cy="12" r="5" fill="none" stroke="currentColor" stroke-width="2"/><line x1="12" y1="1" x2="12" y2="3" stroke="currentColor" stroke-width="2"/><line x1="12" y1="21" x2="12" y2="23" stroke="currentColor" stroke-width="2"/><line x1="4.22" y1="4.22" x2="5.64" y2="5.64" stroke="currentColor" stroke-width="2"/><line x1="18.36" y1="18.36" x2="19.78" y2="19.78" stroke="currentColor" stroke-width="2"/><line x1="1" y1="12" x2="3" y2="12" stroke="currentColor" stroke-width="2"/><line x1="21" y1="12" x2="23" y2="12" stroke="currentColor" stroke-width="2"/><line x1="4.22" y1="19.78" x2="5.64" y2="18.36" stroke="currentColor" stroke-width="2"/><line x1="18.36" y1="5.64" x2="19.78" y2="4.22" stroke="currentColor" stroke-width="2"/>';
    }
    setTimeout(initCharts, 100);
}

// ═══ LIVE CLOCK ═══
function updateClock() {
    const el = document.getElementById('liveClock');
    if (!el) return;
    const n = new Date(), h = n.getHours(), a = h >= 12 ? 'PM' : 'AM',
        h12 = h % 12 || 12, m = String(n.getMinutes()).padStart(2, '0'),
        s = String(n.getSeconds()).padStart(2, '0');
    el.textContent = `${String(h12).padStart(2, '0')}:${m}:${s} ${a}`;
}
setInterval(updateClock, 1000);
updateClock();

// ═══ NOTIFICATIONS ═══
function toggleNotifications() {
    document.getElementById('notifDropdown').classList.toggle('active');
}
document.addEventListener('click', e => {
    if (!e.target.closest('.header-btn') && !e.target.closest('.notif-dropdown'))
        document.getElementById('notifDropdown')?.classList.remove('active');
});

// ═══ MODAL ═══
function openModal(id) { document.getElementById(id).classList.add('active'); }
function closeModal(id) { document.getElementById(id).classList.remove('active'); }

// ═══ SETTINGS TAB ═══
function switchSettingsTab(el) {
    document.querySelectorAll('.settings-nav-item').forEach(i => i.classList.remove('active'));
    el.classList.add('active');
}

// ═══ CHART HELPERS ═══
function isDark() { return document.documentElement.getAttribute('data-theme') === 'dark'; }
function gridColor() { return isDark() ? 'rgba(148,163,184,0.1)' : 'rgba(148,163,184,0.15)'; }
function txtColor() { return isDark() ? '#94A3B8' : '#64748B'; }

const chartInstances = {};
function makeChart(id, config) {
    const el = document.getElementById(id);
    if (!el) return;
    if (chartInstances[id]) chartInstances[id].destroy();
    chartInstances[id] = new Chart(el, config);
}

function sparkline(id, data, color) {
    makeChart(id, {
        type: 'line',
        data: { labels: data, datasets: [{ data, borderColor: color, borderWidth: 2, fill: true, backgroundColor: color + '18', tension: 0.4, pointRadius: 0 }] },
        options: { responsive: true, maintainAspectRatio: false, scales: { x: { display: false }, y: { display: false } }, plugins: { legend: { display: false }, tooltip: { enabled: false } } }
    });
}

// ═══ DASHBOARD CHARTS ═══
function initCharts() {
    sparkline('spark1', [220, 228, 235, 230, 240, 244, 248], '#3B82F6');
    sparkline('spark2', [198, 205, 210, 208, 215, 210, 213], '#22C55E');
    sparkline('spark3', [22, 20, 25, 18, 15, 20, 18], '#EF4444');
    sparkline('spark4', [10, 14, 12, 15, 18, 14, 17], '#8B5CF6');
    sparkline('spark5', [35, 37, 38, 39, 40, 41, 42.5], '#06B6D4');
    sparkline('spark6', [12, 11, 10, 9.5, 9, 8.5, 8.2], '#F59E0B');
    sparkline('spark7', [6, 8, 5, 10, 7, 9, 12], '#EC4899');

    makeChart('salaryChart', {
        type: 'bar',
        data: {
            labels: ['Feb', 'Mar', 'Apr', 'May', 'Jun', 'Jul', 'Aug', 'Sep', 'Oct', 'Nov', 'Dec', 'Jan'],
            datasets: [{
                label: 'Salary Expense (₹L)', data: [36, 37.5, 38, 38.2, 39, 39.5, 40, 40.2, 41, 41.5, 42, 42.5],
                backgroundColor: ctx => { const g = ctx.chart.ctx.createLinearGradient(0, 0, 0, 300); g.addColorStop(0, '#3B82F6'); g.addColorStop(1, '#6366F1'); return g; },
                borderRadius: 8, borderSkipped: false, barPercentage: 0.55
            }]
        },
        options: {
            responsive: true, maintainAspectRatio: false,
            scales: { x: { grid: { display: false }, ticks: { color: txtColor(), font: { size: 12 } } }, y: { grid: { color: gridColor() }, ticks: { color: txtColor(), font: { size: 12 }, callback: v => '₹' + v + 'L' }, border: { display: false } } },
            plugins: { legend: { display: false }, tooltip: { backgroundColor: isDark() ? '#1E293B' : '#fff', titleColor: isDark() ? '#F1F5F9' : '#0F172A', bodyColor: isDark() ? '#94A3B8' : '#64748B', borderColor: isDark() ? '#334155' : '#E2E8F0', borderWidth: 1, cornerRadius: 10, padding: 12, callbacks: { label: ctx => '₹' + ctx.parsed.y + ' Lakhs' } } }
        }
    });

    const dLabels = window.deptLabels && window.deptLabels.length > 0 ? window.deptLabels : ['Engineering', 'Marketing', 'Sales', 'Finance', 'HR', 'Operations'];
    const dData = window.deptData && window.deptData.length > 0 ? window.deptData : [68, 32, 45, 28, 22, 53];
    const dSegmentColors = ['#3B82F6', '#EC4899', '#10B981', '#F59E0B', '#8B5CF6', '#06B6D4', '#6366F1', '#F43F5E', '#14B8A6', '#84CC16'];


    makeChart('deptChart', {
        type: 'doughnut',
        data: {
            labels: dLabels,
            datasets: [{
                data: dData,
                backgroundColor: dSegmentColors,
                borderColor: isDark() ? '#1e293b' : '#ffffff',
                borderWidth: 3,
                hoverOffset: 16,
                hoverBorderWidth: 0
            }]
        },
        options: {
            responsive: true,
            maintainAspectRatio: false,
            cutout: '55%',
            plugins: {
                legend: {
                    display: true,
                    position: 'bottom',
                    labels: {
                        color: txtColor(),
                        font: { size: 13, weight: '600' },
                        padding: 20,
                        usePointStyle: true,
                        pointStyleWidth: 10,
                        generateLabels: (chart) => {
                            const data = chart.data;
                            const total = data.datasets[0].data.reduce((a, b) => a + b, 0);
                            return data.labels.map((label, i) => ({
                                text: `${label}  (${data.datasets[0].data[i]})`,
                                fillStyle: data.datasets[0].backgroundColor[i],
                                strokeStyle: data.datasets[0].backgroundColor[i],
                                pointStyle: 'circle',
                                hidden: false,
                                index: i
                            }));
                        }
                    }
                },
                tooltip: {
                    backgroundColor: isDark() ? '#1E293B' : '#fff',
                    titleColor: isDark() ? '#F1F5F9' : '#0F172A',
                    bodyColor: isDark() ? '#94A3B8' : '#64748B',
                    borderColor: isDark() ? '#334155' : '#E2E8F0',
                    borderWidth: 1,
                    cornerRadius: 10,
                    padding: 12,
                    callbacks: {
                        label: (ctx) => {
                            const total = ctx.dataset.data.reduce((a, b) => a + b, 0);
                            const pct = total > 0 ? Math.round((ctx.parsed / total) * 100) : 0;
                            return `  ${ctx.label}: ${ctx.parsed} employees (${pct}%)`;
                        }
                    }
                }
            }
        }
    });

    makeChart('attendanceChart', {
        type: 'line',
        data: {
            labels: ['Mon', 'Tue', 'Wed', 'Thu', 'Fri', 'Sat', 'Sun'],
            datasets: [
                { label: 'Present', data: [215, 220, 213, 218, 210, 142, 50], borderColor: '#22C55E', backgroundColor: 'rgba(34,197,94,0.08)', fill: true, tension: 0.4, borderWidth: 2.5, pointRadius: 4, pointBackgroundColor: '#22C55E' },
                { label: 'Absent', data: [18, 15, 20, 16, 22, 30, 45], borderColor: '#EF4444', backgroundColor: 'rgba(239,68,68,0.05)', fill: true, tension: 0.4, borderWidth: 2.5, pointRadius: 4, pointBackgroundColor: '#EF4444' },
                { label: 'Late', data: [12, 10, 15, 8, 14, 6, 3], borderColor: '#F59E0B', backgroundColor: 'rgba(245,158,11,0.05)', fill: true, tension: 0.4, borderWidth: 2.5, pointRadius: 4, pointBackgroundColor: '#F59E0B' }
            ]
        },
        options: {
            responsive: true, maintainAspectRatio: false,
            scales: { x: { grid: { display: false }, ticks: { color: txtColor(), font: { size: 12 } } }, y: { grid: { color: gridColor() }, ticks: { color: txtColor(), font: { size: 12 } }, border: { display: false } } },
            plugins: { legend: { display: true, position: 'top', align: 'end', labels: { color: txtColor(), font: { size: 12, weight: 500 }, usePointStyle: true, pointStyleWidth: 8, padding: 20 } } }
        }
    });
}

// ═══ TAX CHART (Payroll) ═══
function initTaxChart() {
    window._taxChartDone = true;
    makeChart('taxChart', {
        type: 'doughnut',
        data: { labels: ['Income Tax', 'Provident Fund', 'ESI', 'Professional Tax'], datasets: [{ data: [2.8, 1.8, 0.9, 1.3], backgroundColor: ['#3B82F6', '#22C55E', '#F59E0B', '#8B5CF6'], borderWidth: 0, hoverOffset: 6 }] },
        options: { responsive: true, maintainAspectRatio: false, cutout: '65%', plugins: { legend: { display: true, position: 'bottom', labels: { color: txtColor(), font: { size: 11, weight: 500 }, usePointStyle: true, pointStyleWidth: 8, padding: 12 } } } }
    });
}

// ═══ REPORT CHART ═══
function initReportChart() {
    window._reportChartDone = true;
    makeChart('reportChart', {
        type: 'bar',
        data: {
            labels: ['Jan', 'Feb', 'Mar', 'Apr', 'May', 'Jun'],
            datasets: [
                { label: 'Attendance %', data: [92, 89, 94, 91, 93, 86], backgroundColor: '#3B82F6', borderRadius: 6, barPercentage: 0.4 },
                { label: 'Payroll (₹L)', data: [42, 38, 40, 39, 41, 42.5], backgroundColor: '#22C55E', borderRadius: 6, barPercentage: 0.4 }
            ]
        },
        options: {
            responsive: true, maintainAspectRatio: false,
            scales: { x: { grid: { display: false }, ticks: { color: txtColor() } }, y: { grid: { color: gridColor() }, ticks: { color: txtColor() }, border: { display: false } } },
            plugins: { legend: { display: true, position: 'top', align: 'end', labels: { color: txtColor(), font: { size: 12, weight: 500 }, usePointStyle: true, pointStyleWidth: 8, padding: 20 } } }
        }
    });
}

// ═══ HEATMAP ═══
function generateHeatmap() {
    const g = document.getElementById('heatmapGrid');
    if (!g) return;
    g.innerHTML = '';
    const l = ['heat-0', 'heat-1', 'heat-2', 'heat-3', 'heat-4', 'heat-5'];
    for (let i = 0; i < 96; i++) {
        const c = document.createElement('div');
        c.className = 'heatmap-cell ' + l[Math.floor(Math.random() * l.length)];
        c.title = `Week ${Math.floor(i / 12) + 1}, Day ${(i % 12) + 1}: ${Math.floor(Math.random() * 15)} late arrivals`;
        g.appendChild(c);
    }
}

// ═══ CALENDAR ═══
function generateCalendar() {
    const cal = document.getElementById('attendanceCalendar');
    if (!cal) return;
    cal.innerHTML = '';
    const days = ['Sun', 'Mon', 'Tue', 'Wed', 'Thu', 'Fri', 'Sat'];
    days.forEach(d => { const c = document.createElement('div'); c.className = 'calendar-header-cell'; c.textContent = d; cal.appendChild(c); });
    const statuses = ['present', 'present', 'present', 'present', 'late', 'absent', 'leave-day', 'present', 'present', ''];
    const colors = { present: '#22C55E', absent: '#EF4444', late: '#F59E0B', 'leave-day': '#8B5CF6' };
    for (let i = 0; i < 3; i++) { const c = document.createElement('div'); c.className = 'calendar-cell empty'; cal.appendChild(c); }
    for (let d = 1; d <= 31; d++) {
        const c = document.createElement('div');
        const s = d <= 15 ? statuses[d % statuses.length] : '';
        c.className = 'calendar-cell ' + s + (d === 15 ? ' today' : '');
        c.innerHTML = `<span>${d}</span>`;
        if (s && colors[s]) c.innerHTML += `<div class="cal-indicator" style="background:${colors[s]}"></div>`;
        cal.appendChild(c);
    }
}

// ═══ CHART TABS ═══
document.querySelectorAll('.chart-tab').forEach(t => {
    t.addEventListener('click', function () {
        this.parentElement.querySelectorAll('.chart-tab').forEach(b => b.classList.remove('active'));
        this.classList.add('active');
    });
});

// ═══ GLOBAL FORM SUBMIT LOADER ═══
function initGlobalFormLoader() {
    document.addEventListener('submit', function (e) {
        const form = e.target;
        if (!form || form.tagName.toLowerCase() !== 'form') return;
        
        let isValid = form.checkValidity();
        if (window.jQuery && $(form).data('validator')) {
            isValid = $(form).valid();
        }

        if (isValid) {
            // Find the button that triggered the submit
            const btn = e.submitter || form.querySelector('button[type="submit"]') || form.querySelector('button:not([type="button"])') || form.querySelector('input[type="submit"]');
            
            if (btn) {
                // Set fixed dimensions using getBoundingClientRect to prevent layout shift
                const rect = btn.getBoundingClientRect();
                btn.style.width = rect.width + 'px';
                btn.style.height = rect.height + 'px';
                
                // Add loading class
                btn.classList.add('is-loading');
                
                // Disable to prevent double submission
                setTimeout(() => {
                    btn.setAttribute('disabled', 'disabled');
                    btn.style.pointerEvents = 'none';
                }, 10);
            }
        }
    });
}

// ═══ GLOBAL LINK LOADER (For "Create" / Navigation Buttons) ═══
function initGlobalLinkLoader() {
    document.addEventListener('click', function (e) {
        // Target any anchor or button with a class starting with 'btn'
        const btn = e.target.closest('a[class*="btn"], button[class*="btn"], .btn');
        if (!btn) return;
        
        // Ignore if opening in a new tab or internal/script link
        if (btn.getAttribute('target') === '_blank') return;
        const href = btn.getAttribute('href');
        if (href && (href === '#' || href.startsWith('javascript:'))) return;
        
        // Ignore if it's a submit button (handled by form loader)
        if (btn.getAttribute('type') === 'submit') return;
        
        // Ignore buttons designed for modals
        if (btn.hasAttribute('data-bs-toggle') || btn.hasAttribute('onclick')) return;

        // Set fixed dimensions
        const rect = btn.getBoundingClientRect();
        btn.style.width = rect.width + 'px';
        btn.style.height = rect.height + 'px';

        // Add loading class
        btn.classList.add('is-loading');
        
        // Safety reset after 8 seconds
        setTimeout(() => {
            btn.classList.remove('is-loading');
            btn.style.width = '';
            btn.style.height = '';
        }, 8000);
    });
}

// ═══ INIT ON DOM LOAD ═══
window.addEventListener('DOMContentLoaded', () => {
    initCharts();
    generateHeatmap();
    generateCalendar();
    initGlobalFormLoader();
    initGlobalLinkLoader();
    if (document.getElementById('taxChart')) initTaxChart();
    if (document.getElementById('reportChart')) initReportChart();
});

// ═══ MODAL CLOSE ON OVERLAY CLICK ═══
document.querySelectorAll('.modal-overlay').forEach(o => {
    o.addEventListener('click', e => { if (e.target === o) o.classList.remove('active'); });
});


// ═══ TOAST SYSTEM ═══
function showToast(message, type = 'info', duration = 5000) {
    let container = document.getElementById('toast-container');
    if (!container) {
        container = document.createElement('div');
        container.id = 'toast-container';
        document.body.appendChild(container);
    }

    const toast = document.createElement('div');
    toast.className = `toast toast-${type}`;
    
    const icons = {
        success: 'fa-check-circle',
        error: 'fa-exclamation-circle',
        warning: 'fa-exclamation-triangle',
        info: 'fa-info-circle'
    };
    
    const titles = {
        success: 'Success',
        error: 'Error',
        warning: 'Warning',
        info: 'Notification'
    };

    toast.innerHTML = `
        <div class="toast-icon">
            <i class="fas ${icons[type]}"></i>
        </div>
        <div class="toast-content">
            <div class="toast-title">${titles[type]}</div>
            <div class="toast-message">${message}</div>
        </div>
        <div class="toast-close">
            <i class="fas fa-times"></i>
        </div>
        <div class="toast-progress">
            <div class="toast-progress-bar"></div>
        </div>
    `;

    container.appendChild(toast);

    // Trigger animation
    setTimeout(() => toast.classList.add('show'), 10);

    // Progress bar animation
    const progressBar = toast.querySelector('.toast-progress-bar');
    progressBar.style.transition = `transform ${duration}ms linear`;
    setTimeout(() => progressBar.style.transform = 'scaleX(0)', 10);

    // Auto remove
    const timer = setTimeout(() => {
        removeToast(toast);
    }, duration);

    // Close button
    toast.querySelector('.toast-close').onclick = () => {
        clearTimeout(timer);
        removeToast(toast);
    };
}

function removeToast(toast) {
    toast.classList.remove('show');
    toast.style.transform = 'translateX(120%)';
    setTimeout(() => {
        if (toast.parentElement) {
            toast.remove();
        }
    }, 400);
}
