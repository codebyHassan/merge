/**
 * AlignHR Core — Centralized JavaScript Module
 * All UI behavior is driven by data-* attributes. Zero inline JS in views.
 */

const AlignHR = {};

/* ============================================================
   AlignHR.Sidebar — toggle, active state, mobile drawer
   ============================================================ */
AlignHR.Sidebar = {
  STORAGE_KEY: 'ahr.sidebar.collapsed',
  OPEN_CLASS:  'ahr-sidebar-open',
  BG_CLASS:    'ahr-backdrop-show',
  COL_CLASS:   'sidebar-collapsed',

  init() {
    this._shell    = document.querySelector('.erp-shell');
    this._sidebar  = document.getElementById('erpSidebar');
    this._backdrop = document.querySelector('.sidebar-backdrop');

    if (!this._sidebar) return;

    // Restore collapsed state on desktop
    if (window.innerWidth >= 992 && localStorage.getItem(this.STORAGE_KEY) === '1') {
      this._shell?.classList.add(this.COL_CLASS);
    }

    // Mobile toggle (hamburger in topbar)
    document.querySelectorAll('[data-erp-sidebar-toggle]').forEach(btn =>
      btn.addEventListener('click', () => this.openMobile())
    );

    // Desktop collapse toggle
    document.querySelectorAll('[data-erp-sidebar-collapse]').forEach(btn =>
      btn.addEventListener('click', () => this.toggleCollapse())
    );

    // Close via backdrop or close button
    this._backdrop?.addEventListener('click', () => this.closeMobile());
    document.querySelectorAll('[data-erp-sidebar-close]').forEach(btn =>
      btn.addEventListener('click', () => this.closeMobile())
    );

    // Mark active link
    this._setActiveLink();
  },

  openMobile() {
    this._sidebar.classList.add(this.OPEN_CLASS);
    this._backdrop?.classList.add(this.BG_CLASS);
    document.body.style.overflow = 'hidden';
  },

  closeMobile() {
    this._sidebar.classList.remove(this.OPEN_CLASS);
    this._backdrop?.classList.remove(this.BG_CLASS);
    document.body.style.overflow = '';
  },

  toggleCollapse() {
    const collapsed = this._shell?.classList.toggle(this.COL_CLASS);
    localStorage.setItem(this.STORAGE_KEY, collapsed ? '1' : '0');
  },

  _setActiveLink() {
    const path = window.location.pathname.toLowerCase();
    // Server-side already sets .active via IsActive(). The JS only does two things:
    //  1. Wipe any duplicate active classes — keep only the BEST match
    //  2. Expand the parent collapse for whichever link is active
    //
    // Bug we are avoiding: `path.startsWith(linkPath)` without a '/' boundary
    // wrongly matches /EmployeeShifts when the linkPath is /Employees, etc.

    const candidates = [];
    this._sidebar.querySelectorAll('a.sidebar-link, a.sidebar-sublink').forEach(a => {
      if (!a.href) return;
      const linkPath = new URL(a.href, window.location.origin).pathname.toLowerCase().replace(/\/+$/, '');
      if (!linkPath || linkPath === '/') return;

      // Exact match OR URL has linkPath followed by a '/' (real path segment boundary)
      const exact   = path === linkPath;
      const isChild = path.startsWith(linkPath + '/');
      if (exact || isChild) {
        candidates.push({ el: a, linkPath, score: exact ? 1000 + linkPath.length : linkPath.length });
      }
    });

    // Pick only the longest (most specific) match. Remove .active from everything else.
    if (candidates.length) {
      candidates.sort((a, b) => b.score - a.score);
      const winner = candidates[0].el;

      this._sidebar.querySelectorAll('.active').forEach(el => el.classList.remove('active'));
      winner.classList.add('active');

      const submenu = winner.closest('.sidebar-submenu');
      if (submenu) {
        submenu.classList.add('show');
        const toggle = document.querySelector(`[data-bs-target="#${submenu.id}"]`);
        toggle?.setAttribute('aria-expanded', 'true');
      }
    }
  }
};

/* ============================================================
   AlignHR.Tables — DataTables init, search, export
   ============================================================ */
AlignHR.Tables = {
  _instances: {},

  init() {
    // Standard DataTables (data-ahr-datatable)
    document.querySelectorAll('[data-ahr-datatable]').forEach(table => {
      if (!table.id) table.id = 'ahr-dt-' + Math.random().toString(36).slice(2);
      this._initDataTable(table);
    });

    // Legacy erp-table search (data-erp-table)
    document.querySelectorAll('[data-erp-table]').forEach(shell => {
      this._initLegacySearch(shell);
    });

    // Select-all checkboxes
    this._initBulkSelect();

    // Export buttons
    document.querySelectorAll('[data-ahr-export]').forEach(btn => {
      btn.addEventListener('click', () => this._handleExport(btn));
    });
  },

  _initDataTable(table) {
    if (typeof $.fn === 'undefined' || typeof $.fn.DataTable === 'undefined') return;

    const pageLen  = parseInt(table.dataset.pageLen  || '10');
    const order    = JSON.parse(table.dataset.order   || '[[0,"asc"]]');
    const searching = table.dataset.searching !== 'false';

    const dt = $(table).DataTable({
      pageLength: pageLen,
      order: order,
      searching: searching,
      language: {
        search:         '',
        searchPlaceholder: 'Search…',
        lengthMenu:     '_MENU_ per page',
        info:           'Showing _START_–_END_ of _TOTAL_',
        paginate: { previous: '‹', next: '›' }
      },
      dom: '<"d-flex align-items-center justify-content-between mb-3"lf>t<"d-flex align-items-center justify-content-between mt-3"ip>',
      responsive: true
    });

    this._instances[table.id] = dt;

    // Wire external search input
    const searchInput = document.querySelector(`[data-dt-search="${table.id}"]`);
    if (searchInput) {
      searchInput.addEventListener('input', () => dt.search(searchInput.value).draw());
    }

    return dt;
  },

  _initLegacySearch(shell) {
    const searchInput = shell.querySelector('[erp-table-search]');
    const filterSel   = shell.querySelector('[erp-table-filter]');
    const tbody       = shell.querySelector('tbody');
    const emptyState  = shell.querySelector('[erp-table-empty]');

    const filter = () => {
      if (!tbody) return;
      const q      = (searchInput?.value || '').toLowerCase();
      const status = filterSel?.value.toLowerCase() || '';
      let visible  = 0;

      tbody.querySelectorAll('tr').forEach(row => {
        const text = row.textContent.toLowerCase();
        const rowStatus = (row.dataset.status || '').toLowerCase();
        const matchQ = !q || text.includes(q);
        const matchS = !status || rowStatus === status;
        row.style.display = (matchQ && matchS) ? '' : 'none';
        if (matchQ && matchS) visible++;
      });

      if (emptyState) emptyState.style.display = visible === 0 ? 'flex' : 'none';
    };

    searchInput?.addEventListener('input', filter);
    filterSel?.addEventListener('change', filter);
  },

  _initBulkSelect() {
    document.querySelectorAll('[data-select-all]').forEach(masterCb => {
      const table = masterCb.closest('table');
      if (!table) return;

      masterCb.addEventListener('change', () => {
        table.querySelectorAll('tbody [type="checkbox"]').forEach(cb => {
          cb.checked = masterCb.checked;
        });
        this._updateBulkBar(table);
      });

      table.querySelectorAll('tbody [type="checkbox"]').forEach(cb => {
        cb.addEventListener('change', () => this._updateBulkBar(table));
      });
    });
  },

  _updateBulkBar(table) {
    const checked = table.querySelectorAll('tbody [type="checkbox"]:checked').length;
    const bar = document.querySelector('.bulk-action-bar');
    if (!bar) return;

    bar.classList.toggle('show', checked > 0);
    const countEl = bar.querySelector('.bulk-count');
    if (countEl) countEl.textContent = `${checked} selected`;
  },

  _handleExport(btn) {
    const type   = btn.dataset.ahrExport;
    const table  = document.querySelector(btn.dataset.target || 'table');
    if (!table) return;

    if (type === 'csv') this._exportCsv(table);
  },

  _exportCsv(table) {
    const rows = [];
    table.querySelectorAll('tr').forEach(row => {
      const cells = [...row.querySelectorAll('th,td')]
        .filter(cell => !cell.querySelector('[type="checkbox"]'))
        .map(cell => `"${cell.textContent.trim().replace(/"/g, '""')}"`);
      rows.push(cells.join(','));
    });
    const blob = new Blob([rows.join('\n')], { type: 'text/csv' });
    const url  = URL.createObjectURL(blob);
    const a    = document.createElement('a');
    a.href     = url;
    a.download = 'export.csv';
    a.click();
    URL.revokeObjectURL(url);
  }
};

/* ============================================================
   AlignHR.Charts — Chart.js wrappers for all dashboard graphs
   ============================================================ */
AlignHR.Charts = {
  DEFAULTS: {
    color:       '#6B7280',
    gridColor:   'rgba(17,24,39,0.05)',
    fontFamily:  "'Inter', sans-serif",
    fontSize:    11,
    accentColor: '#111827',
    mutedColor:  '#E5E7EB',
  },

  init() {
    if (typeof Chart === 'undefined') return;

    Chart.defaults.color          = this.DEFAULTS.color;
    Chart.defaults.font.family    = this.DEFAULTS.fontFamily;
    Chart.defaults.font.size      = this.DEFAULTS.fontSize;
    Chart.defaults.borderColor    = this.DEFAULTS.gridColor;

    document.querySelectorAll('canvas[data-chart-type]').forEach(canvas => {
      this._initChart(canvas);
    });

    // Named charts (legacy)
    this._initAttendanceChart();
    this._initModuleChart();
  },

  _initChart(canvas) {
    const type    = canvas.dataset.chartType;
    const labels  = JSON.parse(canvas.dataset.labels  || '[]');
    const values  = JSON.parse(canvas.dataset.values  || '[]');
    const label   = canvas.dataset.label || 'Data';

    if (!type || !labels.length) return;

    const config = this._buildConfig(type, labels, values, label);
    if (config) new Chart(canvas, config);
  },

  _buildConfig(type, labels, values, label) {
    const palette = this._generatePalette(values.length);

    const base = {
      options: {
        responsive: true,
        maintainAspectRatio: false,
        plugins: {
          legend: { display: type !== 'bar' && type !== 'line', labels: { color: '#6B7280', padding: 10, boxWidth: 8, font: { size: 11 } } },
          tooltip: {
            backgroundColor: '#111827',
            borderColor: '#E5E7EB',
            borderWidth: 0,
            titleColor: '#FFFFFF',
            bodyColor: '#D1D5DB',
            padding: 8,
            cornerRadius: 6,
            titleFont: { size: 12 },
            bodyFont: { size: 11 }
          }
        }
      }
    };

    if (type === 'bar' || type === 'line') {
      base.options.scales = {
        x: { grid: { color: this.DEFAULTS.gridColor, drawBorder: false }, ticks: { color: '#9CA3AF', font: { size: 11 } } },
        y: { grid: { color: this.DEFAULTS.gridColor, drawBorder: false }, ticks: { color: '#9CA3AF', font: { size: 11 } }, beginAtZero: true }
      };
    }

    if (type === 'bar') {
      return { type: 'bar', data: { labels, datasets: [{ label, data: values, backgroundColor: '#111827', borderRadius: 4, borderSkipped: false, maxBarThickness: 28 }] }, options: base.options };
    }

    if (type === 'line') {
      return { type: 'line', data: { labels, datasets: [{ label, data: values, borderColor: '#111827', backgroundColor: 'rgba(17,24,39,0.06)', fill: true, tension: 0.35, pointBackgroundColor: '#111827', pointRadius: 2.5, borderWidth: 1.5 }] }, options: base.options };
    }

    if (type === 'doughnut' || type === 'pie') {
      return { type, data: { labels, datasets: [{ data: values, backgroundColor: palette, borderColor: '#FFFFFF', borderWidth: 2 }] }, options: { ...base.options, cutout: type === 'doughnut' ? '70%' : 0 } };
    }

    return null;
  },

  _generatePalette(n) {
    /* Neutral grayscale only — no orange, no blue, no green */
    const base = ['#111827', '#4B5563', '#9CA3AF', '#D1D5DB', '#E5E7EB', '#F3F4F6'];
    const out = [];
    for (let i = 0; i < n; i++) out.push(base[i % base.length]);
    return out;
  },

  _initAttendanceChart() {
    const canvas = document.getElementById('attendanceChart');
    if (!canvas || typeof Chart === 'undefined') return;

    new Chart(canvas, {
      type: 'bar',
      data: {
        labels: ['Mon', 'Tue', 'Wed', 'Thu', 'Fri', 'Sat', 'Sun'],
        datasets: [
          { label: 'Present', data: [85, 88, 82, 90, 87, 40, 10], backgroundColor: '#111827', borderRadius: 3, borderSkipped: false, maxBarThickness: 22 },
          { label: 'Late',    data: [8,  6,  10, 5,  7,  3,  1],  backgroundColor: '#9CA3AF', borderRadius: 3, borderSkipped: false, maxBarThickness: 22 },
          { label: 'Absent',  data: [7,  6,  8,  5,  6,  7,  89], backgroundColor: '#E5E7EB', borderRadius: 3, borderSkipped: false, maxBarThickness: 22 }
        ]
      },
      options: {
        responsive: true,
        maintainAspectRatio: false,
        scales: {
          x: { stacked: true, grid: { display: false }, ticks: { color: '#9CA3AF', font: { size: 11 } } },
          y: { stacked: true, grid: { color: 'rgba(17,24,39,0.05)', drawBorder: false }, ticks: { color: '#9CA3AF', font: { size: 11 } }, beginAtZero: true }
        },
        plugins: {
          legend: { labels: { color: '#6B7280', padding: 8, boxWidth: 8, font: { size: 11 } } },
          tooltip: { backgroundColor: '#111827', borderWidth: 0, titleColor: '#FFFFFF', bodyColor: '#D1D5DB', padding: 8, cornerRadius: 6, titleFont: { size: 12 }, bodyFont: { size: 11 } }
        }
      }
    });
  },

  _initModuleChart() {
    const canvas = document.getElementById('moduleChart');
    if (!canvas || typeof Chart === 'undefined') return;

    new Chart(canvas, {
      type: 'doughnut',
      data: {
        labels: ['Attendance', 'Leave', 'Payroll', 'HR'],
        datasets: [{
          data: [35, 25, 30, 10],
          backgroundColor: ['#111827', '#4B5563', '#9CA3AF', '#E5E7EB'],
          borderColor: '#FFFFFF',
          borderWidth: 2
        }]
      },
      options: {
        responsive: true,
        maintainAspectRatio: false,
        cutout: '70%',
        plugins: {
          legend: { position: 'bottom', labels: { color: '#6B7280', padding: 8, boxWidth: 8, font: { size: 11 } } },
          tooltip: { backgroundColor: '#111827', borderWidth: 0, titleColor: '#FFFFFF', bodyColor: '#D1D5DB', padding: 8, cornerRadius: 6 }
        }
      }
    });
  }
};

/* ============================================================
   AlignHR.Loading — single pipeline for all loading/busy state
   ============================================================ */
AlignHR.Loading = {
  _busy: false,
  _bar:  null,

  init() {
    // Create top-bar progress indicator
    this._bar = document.createElement('div');
    this._bar.id = 'ahr-page-bar';
    this._bar.setAttribute('aria-hidden', 'true');
    document.body.prepend(this._bar);

    // Show bar on any browser navigation away (postback, link follow, reload)
    window.addEventListener('beforeunload', () => this._showBar());

    // Reset everything on every pageshow — covers initial load, reload, bfcache restore
    window.addEventListener('pageshow', () => {
      this._busy = false;
      this._hideBar();
      document.querySelectorAll('.btn.is-loading').forEach(b => {
        b.classList.remove('is-loading');
        b.disabled = false;
      });
    });
  },

  // Try to acquire the lock. Returns false (and does nothing) if already busy.
  acquire(btn) {
    if (this._busy) return false;
    this._busy = true;
    this._showBar();
    if (btn) btn.classList.add('is-loading');
    return true;
  },

  // Release the lock (used by async operations like file export).
  release(btn) {
    this._busy = false;
    this._hideBar();
    if (btn) btn.classList.remove('is-loading');
  },

  isBusy() { return this._busy; },

  _showBar() { this._bar?.classList.add('ahr-active'); },
  _hideBar()  { this._bar?.classList.remove('ahr-active'); }
};

/* ============================================================
   AlignHR.Forms — validation, loading states, AJAX helpers
   ============================================================ */
AlignHR.Forms = {
  init() {
    // Bootstrap validation
    document.querySelectorAll('form[data-ahr-validate]').forEach(form => {
      form.addEventListener('submit', e => {
        if (!form.checkValidity()) {
          e.preventDefault();
          e.stopPropagation();
        }
        form.classList.add('was-validated');
      });
    });

    // 1. Form submit → route through the loading pipeline
    document.body.addEventListener('submit', (e) => {
      if (e.defaultPrevented) return;
      const form = e.target;
      if (!(form instanceof HTMLFormElement)) return;
      const btn = e.submitter || form.querySelector('[type="submit"]');
      if (!btn) return;
      if (form.querySelector('.is-invalid')) return;
      if (!AlignHR.Loading.acquire(btn)) { e.preventDefault(); return; }
    });

    // 2. Navigation clicks (.btn links, etc.) → route through the loading pipeline
    document.body.addEventListener('click', (e) => {
      const btn = e.target.closest('.btn');
      if (!btn) return;

      // Bootstrap uses .btn on labels for radio/checkbox button groups. Those
      // are local UI toggles, not navigation, and must not lock the page loader.
      if (!['A', 'BUTTON', 'INPUT'].includes(btn.tagName)) return;

      // Skip non-navigating buttons
      if (btn.dataset.bsToggle) return;
      if (btn.hasAttribute('data-ahr-confirm')) return;
      if (btn.hasAttribute('hx-get') || btn.hasAttribute('hx-post')) return;
      if (btn.type === 'button') return;
      if (btn.getAttribute('role') === 'tab') return;
      if (btn.tagName === 'BUTTON' && btn.type !== 'submit') return;

      if (btn.tagName === 'A') {
        const href = btn.getAttribute('href');
        if (!href || href === '#' || href === 'javascript:void(0)' || href.startsWith('#')) return;
        // target="_blank" opens in a new tab — current page never navigates, so the lock would never release
        if (btn.getAttribute('target') === '_blank') return;
      }

      // submit buttons inside forms are handled by the submit listener above
      if (btn.type === 'submit' && btn.closest('form')) return;

      if (!AlignHR.Loading.acquire(btn)) { e.preventDefault(); return; }
    });

    // 3. HTMX requests triggered directly by hx-* attributes (not via the click handler above)
    document.body.addEventListener('htmx:beforeRequest', (e) => {
      const el  = e.detail?.elt;
      if (!el) return;
      const btn = el.tagName === 'BUTTON' ? el
                : el.tagName === 'FORM'   ? el.querySelector('[type="submit"]')
                                          : null;
      if (btn) AlignHR.Loading.acquire(btn);
    });

    const releaseHtmx = (e) => {
      const el  = e.detail?.elt;
      if (!el) return;
      const btn = el.tagName === 'BUTTON' ? el
                : el.tagName === 'FORM'   ? el.querySelector('[type="submit"]')
                                          : null;
      if (btn) AlignHR.Loading.release(btn);
    };
    document.body.addEventListener('htmx:afterRequest',  releaseHtmx);
    document.body.addEventListener('htmx:responseError', releaseHtmx);
    document.body.addEventListener('htmx:sendError',     releaseHtmx);
    document.body.addEventListener('htmx:timeout',       releaseHtmx);
    document.body.addEventListener('htmx:abort',         releaseHtmx);

    // Password toggle (data-ahr-toggle-password)
    document.querySelectorAll('[data-ahr-toggle-password]').forEach(btn => {
      btn.addEventListener('click', () => {
        const target = document.getElementById(btn.dataset.ahrTogglePassword);
        if (!target) return;
        const isText = target.type === 'text';
        target.type = isText ? 'password' : 'text';
        const icon = btn.querySelector('i');
        if (icon) {
          icon.className = isText ? 'bi bi-eye' : 'bi bi-eye-slash';
        }
      });
    });

    // Character counter (data-ahr-maxlength)
    document.querySelectorAll('[data-ahr-maxlength]').forEach(input => {
      const max = parseInt(input.dataset.ahrMaxlength);
      const counter = document.createElement('div');
      counter.className = 'form-text text-end mt-1';
      counter.textContent = `0 / ${max}`;
      input.parentNode.insertBefore(counter, input.nextSibling);
      input.addEventListener('input', () => {
        counter.textContent = `${input.value.length} / ${max}`;
        counter.style.color = input.value.length > max ? '#C87070' : '';
      });
    });
  }
};

/* ============================================================
   AlignHR.Toasts — unified notification system
   ============================================================ */
AlignHR.Toasts = {
  _host: null,
  _counter: 0,

  init() {
    this._host = document.getElementById('erpToastHost');
    if (!this._host) {
      this._host = document.createElement('div');
      this._host.id = 'erpToastHost';
      this._host.className = 'toast-container position-fixed top-0 end-0 p-3';
      this._host.setAttribute('aria-live', 'polite');
      this._host.setAttribute('aria-atomic', 'true');
      document.body.appendChild(this._host);
    }

    // Read TempData from body data attributes
    const body = document.body;
    const successMsg = body.dataset.toastSuccess;
    const errorMsg   = body.dataset.toastError;
    const warningMsg = body.dataset.toastWarning;

    if (successMsg) this.show(successMsg, 'success');
    if (errorMsg)   this.show(errorMsg,   'error');
    if (warningMsg) this.show(warningMsg, 'warning');

    // Demo button
    document.querySelectorAll('[data-erp-toast-demo]').forEach(btn => {
      btn.addEventListener('click', () => this.show('System notification: feature is operational.', 'info'));
    });
  },

  show(message, type = 'info', duration = 5000) {
    if (!message || !this._host) return;
    this._counter++;

    const icons = { success: 'bi-check-circle', error: 'bi-exclamation-circle', warning: 'bi-exclamation-triangle', info: 'bi-info-circle' };
    const labels = { success: 'Success', error: 'Error', warning: 'Warning', info: 'Notice' };

    const id = `ahr-toast-${this._counter}`;
    const toastEl = document.createElement('div');
    toastEl.id = id;
    toastEl.className = `toast ahr-toast-${type}`;
    toastEl.setAttribute('role', 'alert');
    toastEl.setAttribute('aria-live', 'assertive');
    toastEl.setAttribute('aria-atomic', 'true');
    toastEl.innerHTML = `
      <div class="toast-header">
        <i class="bi ${icons[type] || icons.info} me-2"></i>
        <strong class="me-auto">${labels[type] || 'Notice'}</strong>
        <button type="button" class="btn-close" data-bs-dismiss="toast" aria-label="Close"></button>
      </div>
      <div class="toast-body">${message}</div>
    `;

    this._host.appendChild(toastEl);

    if (typeof bootstrap !== 'undefined') {
      const toast = new bootstrap.Toast(toastEl, { delay: duration, autohide: true });
      toast.show();
      toastEl.addEventListener('hidden.bs.toast', () => toastEl.remove());
    }
  }
};

/* ============================================================
   AlignHR.Modals — confirm dialogs, dynamic loader
   ============================================================ */
AlignHR.Modals = {
  init() {
    // Confirm action modals (data-ahr-confirm)
    document.querySelectorAll('[data-ahr-confirm]').forEach(trigger => {
      trigger.addEventListener('click', e => {
        e.preventDefault();
        const message = trigger.dataset.ahrConfirm || 'Are you sure you want to proceed?';
        const href    = trigger.href || trigger.dataset.href;
        const form    = trigger.closest('form');

        this.confirm(message, () => {
          if (href) window.location.href = href;
          else if (form) form.submit();
        });
      });
    });

    // Bootstrap confirmation modal (existing #confirmModal)
    const confirmModal = document.getElementById('confirmModal');
    if (confirmModal) {
      confirmModal.addEventListener('show.bs.modal', event => {
        const trigger  = event.relatedTarget;
        const message  = trigger?.dataset.bsMessage || 'Are you sure?';
        const form     = trigger?.dataset.bsForm;
        const msgEl    = confirmModal.querySelector('[data-confirm-message]');
        const actionBtn = confirmModal.querySelector('[data-confirm-action]');

        if (msgEl) msgEl.textContent = message;

        if (actionBtn && form) {
          actionBtn.onclick = () => {
            document.getElementById(form)?.submit();
          };
        } else if (actionBtn && trigger?.href) {
          actionBtn.onclick = () => { window.location.href = trigger.href; };
        }
      });
    }
  },

  confirm(message, onConfirm, onCancel) {
    const existing = document.getElementById('ahr-confirm-overlay');
    existing?.remove();

    const overlay = document.createElement('div');
    overlay.id = 'ahr-confirm-overlay';
    overlay.innerHTML = `
      <div class="modal fade" id="ahrConfirmDialog" tabindex="-1" aria-modal="true" aria-labelledby="ahrConfirmTitle">
        <div class="modal-dialog modal-dialog-centered modal-sm">
          <div class="modal-content">
            <div class="modal-header">
              <h5 class="modal-title" id="ahrConfirmTitle">Confirm Action</h5>
              <button type="button" class="btn-close" data-bs-dismiss="modal" aria-label="Close"></button>
            </div>
            <div class="modal-body">
              <p class="mb-0" style="font-size:15px">${message}</p>
            </div>
            <div class="modal-footer">
              <button type="button" class="btn btn-secondary btn-sm" data-bs-dismiss="modal">Cancel</button>
              <button type="button" class="btn btn-danger btn-sm" id="ahrConfirmOk">Confirm</button>
            </div>
          </div>
        </div>
      </div>
    `;
    document.body.appendChild(overlay);

    const modal = new bootstrap.Modal(document.getElementById('ahrConfirmDialog'));
    modal.show();

    document.getElementById('ahrConfirmOk').addEventListener('click', () => {
      modal.hide();
      onConfirm?.();
    });

    document.getElementById('ahrConfirmDialog').addEventListener('hidden.bs.modal', () => {
      overlay.remove();
      onCancel?.();
    });
  }
};

/* ============================================================
   AlignHR.DatePicker — flatpickr init for all date inputs
   ============================================================ */
AlignHR.DatePicker = {
  init() {
    if (typeof flatpickr === 'undefined') return;

    const darkTheme = {
      appendTo: document.body,
      disableMobile: false
    };

    // Single date
    document.querySelectorAll('[data-ahr-datepicker]').forEach(input => {
      flatpickr(input, {
        ...darkTheme,
        dateFormat: input.dataset.format || 'Y-m-d',
        allowInput: true
      });
    });

    // Date range
    document.querySelectorAll('[data-ahr-daterange]').forEach(input => {
      flatpickr(input, {
        ...darkTheme,
        mode: 'range',
        dateFormat: input.dataset.format || 'Y-m-d',
        allowInput: true
      });
    });

    // Time
    document.querySelectorAll('[data-ahr-timepicker]').forEach(input => {
      flatpickr(input, {
        ...darkTheme,
        enableTime: true,
        noCalendar: true,
        dateFormat: 'H:i',
        time_24hr: true
      });
    });

    // Datetime
    document.querySelectorAll('[data-ahr-datetime]').forEach(input => {
      flatpickr(input, {
        ...darkTheme,
        enableTime: true,
        dateFormat: 'Y-m-d H:i',
        time_24hr: true
      });
    });

    // Auto-detect date inputs with type="date" if not already initialized
    document.querySelectorAll('input[type="date"]:not(.flatpickr-input)').forEach(input => {
      flatpickr(input, { ...darkTheme, dateFormat: 'Y-m-d', allowInput: true });
    });
  }
};

/* ============================================================
   AlignHR.Filters — filter panel toggle, filter chip display
   ============================================================ */
AlignHR.Filters = {
  init() {
    // Toggle filter panels
    document.querySelectorAll('[data-ahr-filter-toggle]').forEach(btn => {
      const targetId = btn.dataset.ahrFilterToggle;
      const panel    = document.getElementById(targetId);
      if (!panel) return;

      btn.addEventListener('click', () => {
        const open = panel.classList.toggle('show');
        btn.setAttribute('aria-expanded', open);
        const icon = btn.querySelector('.bi-funnel, .bi-funnel-fill');
        if (icon) icon.className = open ? 'bi bi-funnel-fill' : 'bi bi-funnel';
      });
    });

    // Clear filter buttons
    document.querySelectorAll('[data-ahr-clear-filters]').forEach(btn => {
      btn.addEventListener('click', () => {
        const form = btn.closest('form') || document.querySelector(btn.dataset.ahrClearFilters);
        if (!form) return;
        form.querySelectorAll('input:not([type="submit"]):not([type="hidden"]):not([type="checkbox"]):not([type="radio"]), select').forEach(el => {
          el.value = '';
        });
        form.querySelectorAll('input[type="checkbox"], input[type="radio"]').forEach(el => { el.checked = false; });
        this._updateFilterChips(form);
      });
    });

    // Live filter chip display
    document.querySelectorAll('[data-ahr-filters]').forEach(form => {
      form.querySelectorAll('input, select').forEach(el => {
        el.addEventListener('change', () => this._updateFilterChips(form));
      });
    });
  },

  _updateFilterChips(form) {
    const chipContainer = form.querySelector('[data-ahr-filter-chips]')
                       || document.querySelector(`[data-ahr-filter-chips="${form.id}"]`);
    if (!chipContainer) return;

    chipContainer.innerHTML = '';
    form.querySelectorAll('input:not([type="submit"]):not([type="hidden"]):not([type="checkbox"]):not([type="radio"]), select').forEach(el => {
      if (!el.value) return;
      const label = form.querySelector(`label[for="${el.id}"]`)?.textContent || el.name;
      const chip  = document.createElement('span');
      chip.className = 'badge bg-secondary me-1';
      chip.textContent = `${label}: ${el.value}`;
      chipContainer.appendChild(chip);
    });
  }
};

/* ============================================================
   AlignHR.Attendance — clock in/out timer
   ============================================================ */
AlignHR.Attendance = {
  _timerInterval: null,
  _startTime: null,

  init() {
    const clockBtn   = document.getElementById('ahrClockBtn');
    const timerEl    = document.getElementById('ahrClockTimer');
    const statusEl   = document.getElementById('ahrClockStatus');
    if (!clockBtn) return;

    clockBtn.addEventListener('click', () => {
      const isIn = clockBtn.dataset.state !== 'in';
      if (isIn) {
        this._startClock(clockBtn, timerEl, statusEl);
      } else {
        this._stopClock(clockBtn, timerEl, statusEl);
      }
    });

    // Resume if timer stored
    const stored = sessionStorage.getItem('ahr.clock.start');
    if (stored) {
      this._startTime = new Date(stored);
      this._startClock(clockBtn, timerEl, statusEl, true);
    }
  },

  _startClock(btn, timerEl, statusEl, resume = false) {
    if (!resume) {
      this._startTime = new Date();
      sessionStorage.setItem('ahr.clock.start', this._startTime.toISOString());
    }

    btn.dataset.state = 'in';
    btn.textContent   = 'Clock Out';
    btn.classList.replace('btn-primary', 'btn-danger');
    if (statusEl) statusEl.textContent = 'Clocked In';

    this._timerInterval = setInterval(() => {
      const diff = new Date() - this._startTime;
      if (timerEl) timerEl.textContent = this._formatDuration(diff);
    }, 1000);
  },

  _stopClock(btn, timerEl, statusEl) {
    clearInterval(this._timerInterval);
    sessionStorage.removeItem('ahr.clock.start');

    btn.dataset.state = 'out';
    btn.textContent   = 'Clock In';
    btn.classList.replace('btn-danger', 'btn-primary');
    if (statusEl) statusEl.textContent = 'Clocked Out';
    if (timerEl)  timerEl.textContent  = '00:00:00';
  },

  _formatDuration(ms) {
    const s = Math.floor(ms / 1000);
    const h = Math.floor(s / 3600).toString().padStart(2, '0');
    const m = Math.floor((s % 3600) / 60).toString().padStart(2, '0');
    const sec = (s % 60).toString().padStart(2, '0');
    return `${h}:${m}:${sec}`;
  }
};

/* ============================================================
   Global helpers (backward compatible — used by existing views)
   ============================================================ */
function showToast(message, type) {
  AlignHR.Toasts.show(message, type);
}

/* ============================================================
   Boot — all modules initialize on DOMContentLoaded
   ============================================================ */
document.addEventListener('DOMContentLoaded', () => {
  AlignHR.Loading.init();   // must be first — all other inits may trigger loading
  AlignHR.Sidebar.init();
  AlignHR.Tables.init();
  AlignHR.Charts.init();
  AlignHR.Forms.init();
  AlignHR.Toasts.init();
  AlignHR.Modals.init();
  AlignHR.DatePicker.init();
  AlignHR.Filters.init();
  AlignHR.Attendance.init();

  // Tooltips
  if (typeof bootstrap !== 'undefined') {
    document.querySelectorAll('[data-bs-toggle="tooltip"]').forEach(el => {
      new bootstrap.Tooltip(el, { trigger: 'hover', placement: el.dataset.bsPlacement || 'top' });
    });
  }

  // Expose globally for external calls
  window.AlignHR = AlignHR;
});
