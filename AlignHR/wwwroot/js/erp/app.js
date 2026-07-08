(function () {
    "use strict";

    const storage = {
        get(key, fallback) {
            try {
                return localStorage.getItem(key) ?? fallback;
            } catch {
                return fallback;
            }
        },
        set(key, value) {
            try {
                localStorage.setItem(key, value);
            } catch {
                // Storage can be unavailable in private or locked-down browsers.
            }
        }
    };

    const qs = (selector, scope = document) => scope.querySelector(selector);
    const qsa = (selector, scope = document) => Array.from(scope.querySelectorAll(selector));

    function syncThemeIcons(theme) {
        qsa("[data-erp-theme-toggle] i, [data-erp-theme-toggle] .bi").forEach((icon) => {
            icon.className = theme === "dark" ? "bi bi-sun" : "bi bi-moon-stars";
        });
    }

    function initTheme() {
        const savedTheme = storage.get("erp.theme", "light");
        document.documentElement.setAttribute("data-bs-theme", savedTheme);
        syncThemeIcons(savedTheme);

        qsa("[data-erp-theme-toggle]").forEach((button) => {
            button.addEventListener("click", () => {
                const currentTheme = document.documentElement.getAttribute("data-bs-theme") || "light";
                const nextTheme = currentTheme === "dark" ? "light" : "dark";
                document.documentElement.setAttribute("data-bs-theme", nextTheme);
                storage.set("erp.theme", nextTheme);
                syncThemeIcons(nextTheme);
            });
        });
    }

    function initSidebar() {
        qsa("[data-erp-sidebar-toggle]").forEach((button) => {
            button.addEventListener("click", () => document.body.classList.add("sidebar-open"));
        });

        qsa("[data-erp-sidebar-close]").forEach((button) => {
            button.addEventListener("click", () => document.body.classList.remove("sidebar-open"));
        });

        qsa("[data-erp-sidebar-collapse]").forEach((button) => {
            button.addEventListener("click", () => {
                document.body.classList.toggle("sidebar-collapsed");
                storage.set("erp.sidebar", document.body.classList.contains("sidebar-collapsed") ? "collapsed" : "expanded");
            });
        });

        if (storage.get("erp.sidebar", "expanded") === "collapsed") {
            document.body.classList.add("sidebar-collapsed");
        }
    }

    function initPermissions() {
        const permissions = new Set((document.body.dataset.permissions || "").split(",").map((item) => item.trim()).filter(Boolean));
        qsa("[data-permission]").forEach((node) => {
            if (!permissions.has(node.dataset.permission)) {
                node.hidden = true;
            }
        });
    }

    function toastIcon(type) {
        return {
            success: "bi-check-circle",
            error: "bi-x-circle",
            warning: "bi-exclamation-triangle",
            info: "bi-info-circle"
        }[type] || "bi-info-circle";
    }

    window.erpToast = function erpToast(message, type = "info", title = "Notification") {
        const host = qs("#erpToastHost");
        if (!host || !window.bootstrap) return;

        const toast = document.createElement("div");
        toast.className = `toast toast-${type}`;
        toast.setAttribute("role", "status");
        toast.setAttribute("aria-live", "polite");
        toast.setAttribute("aria-atomic", "true");
        toast.innerHTML = `
            <div class="toast-header">
                <i class="bi ${toastIcon(type)} me-2"></i>
                <strong class="me-auto">${title}</strong>
                <small>now</small>
                <button type="button" class="btn-close ms-2 mb-1" data-bs-dismiss="toast" aria-label="Close"></button>
            </div>
            <div class="toast-body">${message}</div>
        `;

        host.appendChild(toast);
        const instance = new bootstrap.Toast(toast, { delay: 4200 });
        toast.addEventListener("hidden.bs.toast", () => toast.remove());
        instance.show();
    };

    window.showToast = function showToast(message, type = "info") {
        const title = {
            success: "Success",
            error: "Error",
            warning: "Warning",
            info: "Notification"
        }[type] || "Notification";
        window.erpToast?.(message, type, title);
    };

    function initToasts() {
        qsa("[data-erp-toast-demo]").forEach((button) => {
            button.addEventListener("click", () => {
                window.erpToast("4 items need review: leave approvals, attendance exceptions, payroll draft, and stock alert.", "info", "Notifications");
            });
        });
    }

    function initCommandPalette() {
        const modalElement = qs("#commandPalette");
        const input = qs("#commandSearchInput");
        if (!modalElement || !window.bootstrap) return;

        document.addEventListener("keydown", (event) => {
            const isShortcut = (event.ctrlKey || event.metaKey) && event.key.toLowerCase() === "k";
            if (!isShortcut) return;
            event.preventDefault();
            bootstrap.Modal.getOrCreateInstance(modalElement).show();
        });

        modalElement.addEventListener("shown.bs.modal", () => input?.focus());

        input?.addEventListener("input", () => {
            const term = input.value.toLowerCase().trim();
            qsa(".command-item", modalElement).forEach((item) => {
                item.hidden = term.length > 0 && !item.textContent.toLowerCase().includes(term);
            });
        });
    }

    function initValidation() {
        qsa(".needs-validation").forEach((form) => {
            form.addEventListener("submit", (event) => {
                if (!form.checkValidity()) {
                    event.preventDefault();
                    event.stopPropagation();
                    window.erpToast?.("Please correct the highlighted fields before saving.", "warning", "Validation");
                }
                form.classList.add("was-validated");
            });
        });
    }

    function initAutosave() {
        qsa("form[data-autosave]").forEach((form) => {
            const key = `erp.autosave.${form.dataset.autosave}`;
            const saved = storage.get(key, "");
            if (saved) {
                try {
                    const data = JSON.parse(saved);
                    Object.keys(data).forEach((name) => {
                        const field = form.elements[name];
                        if (field && "value" in field) field.value = data[name];
                    });
                } catch {
                    // Ignore malformed local drafts.
                }
            }

            form.addEventListener("input", () => {
                const data = Object.fromEntries(new FormData(form).entries());
                storage.set(key, JSON.stringify(data));
                const note = qs("[data-autosave-note]", form.closest("[data-wizard]") || form);
                if (note) note.textContent = "Draft autosaved just now";
            });
        });
    }

    function initWizard() {
        qsa("[data-wizard]").forEach((wizard) => {
            let index = 0;
            const steps = qsa(".wizard-step", wizard);
            const indicators = qsa(".wizard-step-indicator", wizard);
            const next = qs("[data-wizard-next]", wizard);
            const prev = qs("[data-wizard-prev]", wizard);

            const render = () => {
                steps.forEach((step, stepIndex) => step.classList.toggle("active", stepIndex === index));
                indicators.forEach((indicator, stepIndex) => indicator.classList.toggle("active", stepIndex <= index));
                if (prev) prev.disabled = index === 0;
                if (next) next.textContent = index === steps.length - 1 ? "Submit request" : "Next";
            };

            next?.addEventListener("click", () => {
                const currentStep = steps[index];
                const fields = qsa("input, select, textarea", currentStep);
                const invalidField = fields.find((field) => !field.checkValidity());
                if (invalidField) {
                    invalidField.reportValidity();
                    return;
                }
                if (index < steps.length - 1) {
                    index += 1;
                    render();
                } else {
                    window.erpToast?.("Leave request submitted for approval.", "success", "Workflow");
                }
            });

            prev?.addEventListener("click", () => {
                index = Math.max(0, index - 1);
                render();
            });

            render();
        });
    }

    function initTables() {
        qsa("[data-erp-table]").forEach((shell) => {
            const table = qs("table", shell);
            const filterContext = shell.previousElementSibling?.classList.contains("filter-bar")
                ? shell.parentElement
                : shell;
            const search = qs("[data-table-search]", filterContext);
            const filter = qs("[data-table-filter]", filterContext);
            const rows = () => qsa("tbody tr", table);

            const updateEmpty = () => {
                const visibleRows = rows().filter((row) => !row.hidden);
                const emptyState = qs("[data-table-empty]", shell);
                if (emptyState) emptyState.style.display = visibleRows.length ? "none" : "block";
            };

            const applyFilters = () => {
                const term = (search?.value || "").toLowerCase().trim();
                const status = filter?.value || "";
                rows().forEach((row) => {
                    const matchesSearch = !term || row.textContent.toLowerCase().includes(term);
                    const matchesStatus = !status || row.dataset.status === status;
                    row.hidden = !(matchesSearch && matchesStatus);
                });
                updateEmpty();
            };

            search?.addEventListener("input", applyFilters);
            filter?.addEventListener("change", applyFilters);

            qsa("[data-sort]", shell).forEach((button) => {
                button.addEventListener("click", () => {
                    const column = Number(button.dataset.sort);
                    const direction = button.dataset.direction === "asc" ? "desc" : "asc";
                    button.dataset.direction = direction;

                    rows()
                        .sort((a, b) => {
                            const aValue = a.children[column]?.textContent.trim() || "";
                            const bValue = b.children[column]?.textContent.trim() || "";
                            return direction === "asc"
                                ? aValue.localeCompare(bValue, undefined, { numeric: true })
                                : bValue.localeCompare(aValue, undefined, { numeric: true });
                        })
                        .forEach((row) => table.tBodies[0].appendChild(row));
                });
            });

            qs("[data-select-all]", shell)?.addEventListener("change", (event) => {
                qsa("tbody input[type='checkbox']", table).forEach((checkbox) => {
                    checkbox.checked = event.target.checked;
                });
            });

            applyFilters();
        });
    }

    function initConfirmations() {
        const modal = qs("#confirmDialog");
        const message = qs("[data-confirm-message]", modal || document);
        const accept = qs("[data-confirm-accept]", modal || document);
        if (!modal || !message || !accept || !window.bootstrap) return;

        qsa("[data-confirm]").forEach((trigger) => {
            trigger.addEventListener("click", () => {
                message.textContent = trigger.dataset.confirm || "Confirm this action?";
                bootstrap.Modal.getOrCreateInstance(modal).show();
            });
        });

        accept.addEventListener("click", () => {
            bootstrap.Modal.getOrCreateInstance(modal).hide();
            window.erpToast?.("Action confirmed. Wire this button to your server workflow.", "info", "Confirmed");
        });
    }

    function crudRoute() {
        const parts = window.location.pathname.split("/").filter(Boolean);
        return {
            controller: parts[0] || "Dashboard",
            action: parts[1] || "Index",
            id: parts[2] || ""
        };
    }

    function moduleTitle(route) {
        const heading = qs(".page-title") || qs(".workspace-header h1");
        return (heading?.textContent || route.controller).trim();
    }

    function moduleSubtitle(route, fallback) {
        const subtitle = qs(".page-subtitle") || qs(".workspace-header .text-muted");
        return (subtitle?.textContent || fallback || `Dedicated ${route.action.toLowerCase()} page for ${route.controller.toLowerCase()}.`).trim();
    }

    function hideLegacyChrome() {
        qsa(".page-header, .breadcrumb-container").forEach((node) => node.classList.add("erp-crud-legacy-hidden"));
    }

    function normalizeActionMenu(cell) {
        if (!cell || qs(".dropdown-menu", cell)) return;
        const actions = qsa("a, form, button", cell).filter((node) => !node.closest(".dropdown"));
        if (!actions.length) return;

        const dropdown = document.createElement("div");
        dropdown.className = "dropdown";
        dropdown.innerHTML = `
            <button class="btn btn-icon btn-sm" type="button" data-bs-toggle="dropdown" aria-label="Open actions">
                <i class="bi bi-three-dots"></i>
            </button>
            <ul class="dropdown-menu dropdown-menu-end"></ul>
        `;
        const menu = qs(".dropdown-menu", dropdown);

        actions.forEach((action) => {
            const li = document.createElement("li");
            if (action.tagName === "FORM") {
                action.style.display = "";
                const button = qs("button", action);
                if (button) {
                    button.className = button.className.includes("danger") || button.className.includes("text-danger")
                        ? "dropdown-item text-danger"
                        : "dropdown-item";
                }
                li.appendChild(action);
            } else if (action.tagName === "A") {
                const text = action.getAttribute("title") || action.textContent.trim() || (action.href.includes("/Edit") ? "Edit" : action.href.includes("/Details") ? "Details" : "Open");
                action.className = action.className.includes("danger") || action.className.includes("text-danger")
                    ? "dropdown-item text-danger"
                    : "dropdown-item";
                action.innerHTML = text;
                li.appendChild(action);
            } else {
                const text = action.getAttribute("title") || action.textContent.trim() || "Action";
                action.className = action.className.includes("danger") || action.className.includes("text-danger")
                    ? "dropdown-item text-danger"
                    : "dropdown-item";
                action.innerHTML = text;
                li.appendChild(action);
            }
            menu.appendChild(li);
        });

        cell.innerHTML = "";
        cell.appendChild(dropdown);
    }

    function normalizeIndexPage(route) {
        if (["dashboard", "home"].includes(route.controller.toLowerCase())) return;
        if (!route.action.equalsIgnoreCase?.("Index") && route.action.toLowerCase() !== "index") return;
        if (qs(".filter-bar") && qs(".table-shell[data-erp-table]")) return;
        const table = qs("main table");
        if (!table || table.dataset.erpCrudNormalized) return;
        table.dataset.erpCrudNormalized = "true";

        hideLegacyChrome();

        let shell = table.closest(".table-shell");
        if (!shell) {
            shell = document.createElement("section");
            shell.className = "table-shell";
            const wrapper = table.closest(".erp-table-wrap") || table.parentElement;
            wrapper.parentNode.insertBefore(shell, wrapper);
            shell.appendChild(wrapper);
        }
        shell.className = "table-shell";
        shell.setAttribute("data-erp-table", "");

        table.className = "table table-hover erp-table";
        const title = moduleTitle(route);
        const entityName = title.replace(/management/ig, "").trim() || route.controller;
        const createLink = qs(`a[href*="/${route.controller}/Create"], a[href$="/Create"], a[href$="/create"]`);
        const createHref = createLink?.getAttribute("href") || `/${route.controller}/Create`;

        const oldSearch = qs("input[name='search'], input[type='search'], .filter-input");
        const filterBar = document.createElement("section");
        filterBar.className = "filter-bar mb-2";
        filterBar.setAttribute("aria-label", `${title} filters`);
        filterBar.innerHTML = `
            <div>
                <h2 class="h6 fw-bold mb-0">${title}</h2>
                <div class="text-muted small">Dedicated list page for ${route.controller.toLowerCase()} records</div>
            </div>
            <div class="filter-group">
                <div class="input-group input-group-sm">
                    <span class="input-group-text"><i class="bi bi-search"></i></span>
                    <input class="form-control" type="search" placeholder="Search ${entityName.toLowerCase()}" data-table-search aria-label="Search records" form="none" />
                </div>
                <select class="form-select form-select-sm" data-table-filter aria-label="Filter by status" form="none">
                    <option value="">All statuses</option>
                    <option value="active">Active</option>
                    <option value="pending">Pending</option>
                    <option value="risk">Risk</option>
                    <option value="approved">Approved</option>
                </select>
                <div class="dropdown">
                    <button class="btn btn-outline-secondary btn-sm dropdown-toggle" type="button" data-bs-toggle="dropdown">Columns</button>
                    <ul class="dropdown-menu dropdown-menu-end">
                        <li><label class="dropdown-item"><input class="form-check-input me-2" type="checkbox" checked />Owner</label></li>
                        <li><label class="dropdown-item"><input class="form-check-input me-2" type="checkbox" checked />Department</label></li>
                        <li><label class="dropdown-item"><input class="form-check-input me-2" type="checkbox" checked />Priority</label></li>
                    </ul>
                </div>
                <button class="btn btn-outline-secondary btn-sm" type="button"><i class="bi bi-funnel me-1"></i>Advanced</button>
                <button class="btn btn-outline-secondary btn-sm" type="button"><i class="bi bi-download me-1"></i>Export</button>
                <a class="btn btn-primary btn-sm" href="${createHref}"><i class="bi bi-plus-lg me-1"></i>Create</a>
            </div>
        `;
        if (oldSearch) {
            const newSearch = qs("[data-table-search]", filterBar);
            newSearch.value = oldSearch.value || "";
            ["hx-get", "hx-target", "hx-select", "hx-indicator", "hx-trigger", "name"].forEach((attr) => {
                if (oldSearch.hasAttribute(attr)) newSearch.setAttribute(attr, oldSearch.getAttribute(attr));
            });
        }
        shell.parentNode.insertBefore(filterBar, shell);

        const rows = qsa("tbody tr", table);
        qsa("thead th", table).forEach((th, index) => {
            th.scope = "col";
            if (index < 3 && !qs(".table-sort", th)) {
                const text = th.textContent.trim();
                th.innerHTML = `<button class="table-sort" type="button" data-sort="${index}">${text}</button>`;
            }
            if (index === qsa("thead th", table).length - 1) {
                th.classList.add("text-end", "sticky-actions");
            }
        });

        const headers = qsa("thead th", table).map((th) => th.textContent.trim() || "Action");
        rows.forEach((row) => {
            const text = row.textContent.toLowerCase();
            const status = text.includes("rejected") || text.includes("inactive") || text.includes("risk")
                ? "risk"
                : text.includes("pending")
                    ? "pending"
                    : text.includes("approved")
                        ? "approved"
                        : "active";
            row.dataset.status = row.dataset.status || status;
            qsa("td", row).forEach((td, index) => {
                td.dataset.label = td.dataset.label || headers[index] || "";
                if (index === row.children.length - 1) {
                    td.classList.add("text-end", "sticky-actions");
                    normalizeActionMenu(td);
                }
            });
        });

        qsa(".table-toolbar", shell).forEach((toolbar) => toolbar.remove());

        let wrap = table.closest(".erp-table-wrap");
        if (!wrap) {
            wrap = document.createElement("div");
            wrap.className = "erp-table-wrap";
            table.parentNode.insertBefore(wrap, table);
            wrap.appendChild(table);
        }

        if (!qs("[data-table-empty]", wrap)) {
            const empty = document.createElement("div");
            empty.className = "empty-state";
            empty.setAttribute("data-table-empty", "");
            empty.innerHTML = `<i class="bi bi-inbox"></i><h3 class="h6 mt-2 mb-1">No records match the current filters</h3><p class="text-muted mb-0">Adjust search, filters, or column criteria.</p>`;
            wrap.appendChild(empty);
        }

        qsa(".pagination-wrapper", shell).forEach((pager) => {
            const footer = document.createElement("div");
            footer.className = "table-footer";
            footer.innerHTML = `<span class="text-muted small fw-semibold">Showing 1-${rows.length} of ${rows.length}</span>`;
            const nav = document.createElement("nav");
            nav.setAttribute("aria-label", `${title} pagination`);
            const pagination = qs(".pagination", pager);
            if (pagination) {
                pagination.classList.add("pagination-sm", "mb-0");
                nav.appendChild(pagination);
            }
            footer.appendChild(nav);
            pager.replaceWith(footer);
        });
    }

    function normalizeFormPage(route) {
        const action = route.action.toLowerCase();
        if (action !== "create" && action !== "edit") return;
        const form = qs("main form[method='post'], main form");
        if (!form || form.dataset.erpCrudNormalized) return;
        if (form.classList.contains("employee-create")) return;
        if (qs(".record-toolbar") && qs(".form-section", form)) return;
        form.dataset.erpCrudNormalized = "true";
        hideLegacyChrome();

        const isEdit = action === "edit";
        const actionLabel = isEdit ? "Update" : "Create";
        const entityName = route.controller.replace(/([a-z])([A-Z])/g, "$1 $2");
        const currentFormId = form.id || "recordForm";
        form.id = currentFormId;

        const shell = form.closest(".form-shell");
        if (shell && shell !== form) {
            shell.parentNode.insertBefore(form, shell);
            shell.remove();
        }
        form.classList.add("form-shell", "needs-validation");
        form.setAttribute("novalidate", "");
        form.dataset.autosave = `${route.controller.toLowerCase()}-record`;

        qsa(".card-inner", form).forEach((inner) => {
            while (inner.firstChild) form.insertBefore(inner.firstChild, inner);
            inner.remove();
        });

        qsa("select.form-control", form).forEach((select) => {
            select.classList.remove("form-control");
            select.classList.add("form-select");
        });

        qsa(".form-grid, .form-row", form).forEach((grid) => {
            grid.className = "row g-2";
            Array.from(grid.children).forEach((child) => {
                if (child.matches("input[type='hidden'], script")) return;
                child.classList.add("col-12", "col-md-6", "col-xl-3");
                if (child.classList.contains("full-width") || child.style.gridColumn) {
                    child.classList.remove("col-xl-3");
                    child.classList.add("col-xl-6");
                }
            });
        });

        let actions = qs(".form-actions-sticky", form) || qs(".card-actions", form);
        const submit = qs("button[type='submit'], input[type='submit']", form);
        const cancel = qs("a[href*='Index'], a[href$='Index'], a.btn-outline-secondary", form);
        if (!actions) {
            actions = document.createElement("div");
            actions.className = "form-actions-sticky";
            form.appendChild(actions);
        }
        actions.className = "form-actions-sticky";
        actions.innerHTML = "";
        actions.innerHTML = `<span class="autosave-note me-auto align-self-center" data-autosave-note>Draft autosave ready</span>`;
        const cancelLink = cancel || document.createElement("a");
        cancelLink.className = "btn btn-outline-secondary btn-sm";
        cancelLink.href = cancelLink.getAttribute("href") || `/${route.controller}`;
        cancelLink.textContent = "Cancel";
        actions.appendChild(cancelLink);
        const draft = document.createElement("button");
        draft.className = "btn btn-outline-secondary btn-sm";
        draft.type = "button";
        draft.textContent = "Save Draft";
        actions.appendChild(draft);
        const submitButton = submit || document.createElement("button");
        submitButton.className = "btn btn-primary btn-sm";
        submitButton.type = "submit";
        submitButton.textContent = `${actionLabel} ${entityName}`;
        actions.appendChild(submitButton);

        const blocks = Array.from(form.children).filter((child) => {
            if (child === actions) return false;
            if (child.matches("input[type='hidden'], script")) return false;
            if (child.classList.contains("form-section")) return false;
            return true;
        });
        const sectionNames = ["Core Information", "Workflow & Approval", "Additional Details"];
        blocks.forEach((block, index) => {
            const section = document.createElement("div");
            section.className = "form-section";
            const id = `${sectionNames[Math.min(index, 2)].toLowerCase().replace(/[^a-z0-9]+/g, "-")}-${index}`;
            const collapsed = index > 1 ? " collapsed" : "";
            const show = index > 1 ? "" : " show";
            section.innerHTML = `
                <button class="form-section-header${collapsed}" type="button" data-bs-toggle="collapse" data-bs-target="#${id}" aria-expanded="${index > 1 ? "false" : "true"}">
                    ${sectionNames[Math.min(index, 2)]} <i class="bi bi-chevron-down"></i>
                </button>
                <div class="collapse${show}" id="${id}">
                    <div class="form-section-body"></div>
                </div>
            `;
            form.insertBefore(section, block);
            qs(".form-section-body", section).appendChild(block);
        });

        const toolbar = document.createElement("section");
        toolbar.className = "record-toolbar mb-2";
        toolbar.innerHTML = `
            <div>
                <h2 class="h6 fw-bold mb-0">${actionLabel} ${entityName}</h2>
                <div class="text-muted small">Full-page CRUD workflow with compact grouped fields and sticky actions.</div>
            </div>
            <div class="page-actions">
                <a class="btn btn-outline-secondary btn-sm" href="/${route.controller}">Cancel</a>
                <button class="btn btn-primary btn-sm" type="submit" form="${currentFormId}">${actionLabel}</button>
            </div>
        `;
        form.parentNode.insertBefore(toolbar, form);
    }

    function normalizeDetailsPage(route) {
        if (route.action.toLowerCase() !== "details") return;
        if (route.controller.toLowerCase() === "employees") return; // Bypass for natively modernized modules
        const workspace = qs("#main-workspace");
        if (!workspace || workspace.dataset.erpCrudDetailsNormalized) return;
        if (qs(".record-toolbar", workspace) && qs(".detail-list", workspace) && qs(".erp-card .nav-tabs", workspace)) return;
        workspace.dataset.erpCrudDetailsNormalized = "true";
        hideLegacyChrome();

        const title = moduleTitle(route);
        const fields = [];
        qsa(".info-group, .info-item, .detail-item, dl div", workspace).forEach((item) => {
            const label = qs("label, span, dt", item)?.textContent.trim();
            const value = qs(".value-highlight, .value, strong, dd", item)?.textContent.trim();
            if (label && value) fields.push({ label, value });
        });
        if (!fields.length) {
            fields.push(
                { label: "Code", value: route.id || "-" },
                { label: "Owner", value: "-" },
                { label: "Department", value: route.controller },
                { label: "Status", value: "Active" }
            );
        }
        while (fields.length < 8) {
            fields.push({ label: ["Priority", "Updated", "Area", "Record Type"][fields.length - 4] || "Record", value: fields.length === 7 ? route.controller : "-" });
        }

        qsa(".details-grid, .erp-card, .details-actions-footer", workspace).forEach((node) => node.classList.add("erp-crud-legacy-hidden"));

        const fragment = document.createElement("div");
        fragment.className = "erp-crud-details";
        fragment.innerHTML = `
            <section class="record-toolbar mb-2">
                <div>
                    <h2 class="h6 fw-bold mb-0">${title}</h2>
                    <div class="text-muted small">${route.controller} details and audit context</div>
                </div>
                <div class="page-actions">
                    <a class="btn btn-outline-secondary btn-sm" href="/${route.controller}">Back to list</a>
                    <a class="btn btn-primary btn-sm" href="/${route.controller}/Edit/${route.id}">Edit</a>
                </div>
            </section>
            <section class="detail-list mb-2">
                ${fields.slice(0, 8).map((field) => `<div class="detail-item"><span>${field.label}</span><strong>${field.value}</strong></div>`).join("")}
            </section>
            <section class="erp-card">
                <ul class="nav nav-tabs" id="detailTabs" role="tablist">
                    <li class="nav-item" role="presentation"><button class="nav-link active" data-bs-toggle="tab" data-bs-target="#summary-tab-pane" type="button" role="tab">Summary</button></li>
                    <li class="nav-item" role="presentation"><button class="nav-link" data-bs-toggle="tab" data-bs-target="#audit-tab-pane" type="button" role="tab">Audit Trail</button></li>
                    <li class="nav-item" role="presentation"><button class="nav-link" data-bs-toggle="tab" data-bs-target="#attachments-tab-pane" type="button" role="tab">Attachments</button></li>
                </ul>
                <div class="tab-content pt-2">
                    <div class="tab-pane fade show active" id="summary-tab-pane" role="tabpanel" tabindex="0">
                        <p class="mb-2">This page represents a focused enterprise details view with compact metadata, related records, audit context, and full-page edit flow.</p>
                        <div class="skeleton mb-2" style="height: 20px"></div>
                        <div class="skeleton" style="height: 20px; width: 72%"></div>
                    </div>
                    <div class="tab-pane fade" id="audit-tab-pane" role="tabpanel" tabindex="0">
                        <div class="timeline">
                            <div class="timeline-item"><span class="timeline-icon"><i class="bi bi-check2"></i></span><div><strong>Record reviewed</strong><small>Latest activity by system user</small></div></div>
                            <div class="timeline-item"><span class="timeline-icon"><i class="bi bi-pencil"></i></span><div><strong>Record updated</strong><small>Previous business day</small></div></div>
                        </div>
                    </div>
                    <div class="tab-pane fade" id="attachments-tab-pane" role="tabpanel" tabindex="0">
                        <div class="empty-state d-block">
                            <i class="bi bi-paperclip"></i>
                            <h3 class="h6 mt-2 mb-1">No attachments</h3>
                            <p class="text-muted mb-0">Documents uploaded for this record will appear here.</p>
                        </div>
                    </div>
                </div>
            </section>
        `;
        const header = qs(".workspace-header", workspace);
        workspace.insertBefore(fragment, header?.nextSibling || workspace.firstChild);
    }

    function initCrudArchitecture() {
        return;
    }

    function initAlignHrCompatibility() {
        qsa("table").forEach((table) => {
            const headers = qsa("thead th", table).map((header) => header.textContent.trim());
            qsa("tbody tr", table).forEach((row) => {
                qsa("td", row).forEach((cell, index) => {
                    if (!cell.dataset.label && headers[index]) {
                        cell.dataset.label = headers[index];
                    }
                });
            });
        });
    }

    function initCharts() {
        if (!window.Chart) return;

        const attendance = qs("#attendanceChart");
        const modules = qs("#moduleChart");
        if (attendance) {
            new Chart(attendance, {
                type: "line",
                data: {
                    labels: ["Mon", "Tue", "Wed", "Thu", "Fri", "Sat"],
                    datasets: [{
                        label: "Attendance",
                        data: [92, 94, 91, 96, 95, 89],
                        tension: 0.38,
                        fill: true,
                        borderColor: "#155eef",
                        backgroundColor: "rgba(21, 94, 239, 0.12)",
                        pointRadius: 3
                    }]
                },
                options: {
                    responsive: true,
                    maintainAspectRatio: false,
                    plugins: { legend: { display: false } },
                    scales: {
                        y: { beginAtZero: false, suggestedMin: 80, suggestedMax: 100 },
                        x: { grid: { display: false } }
                    }
                }
            });
        }

        if (modules) {
            new Chart(modules, {
                type: "doughnut",
                data: {
                    labels: ["HR", "Payroll", "Inventory", "Reports"],
                    datasets: [{
                        data: [38, 24, 22, 16],
                        backgroundColor: ["#155eef", "#079455", "#dc6803", "#0ba5ec"],
                        borderWidth: 0
                    }]
                },
                options: {
                    cutout: "68%",
                    plugins: { legend: { position: "bottom", labels: { usePointStyle: true } } }
                }
            });
        }
    }

    function updateExportUrl() {
        const search = qs("[name='search']")?.value || "";
        const departmentId = qs("[name='departmentId']")?.value || "";
        const subDepartmentId = qs("[name='subDepartmentId']")?.value || "";
        const gradeId = qs("[name='gradeId']")?.value || "";
        const divisionId = qs("[name='divisionId']")?.value || "";
        const status = qs("[name='status']")?.value || "";

        const params = new URLSearchParams();
        if (search) params.append("search", search);
        if (departmentId) params.append("departmentId", departmentId);
        if (subDepartmentId) params.append("subDepartmentId", subDepartmentId);
        if (gradeId) params.append("gradeId", gradeId);
        if (divisionId) params.append("divisionId", divisionId);
        if (status) params.append("status", status);

        const columns = [];
        qsa("[data-column-toggle]:checked").forEach(checkbox => {
            columns.push(checkbox.dataset.columnToggle);
        });
        if (columns.length > 0) {
            params.append("columns", columns.join(","));
        }

        const exportBtn = qs("#exportBtn");
        if (exportBtn) {
            const parts = window.location.pathname.split("/").filter(Boolean);
            const controller = parts[0] || "Employees";
            exportBtn.href = `/${controller}/Export?${params.toString()}`;
        }
    }

    function initColumnVisibility() {
        document.addEventListener("change", (event) => {
            const toggle = event.target.closest("[data-column-toggle]");
            if (!toggle) return;
            const columnName = toggle.dataset.columnToggle;
            const isChecked = toggle.checked;

            const table = qs(".erp-table");
            if (!table) return;

            const headers = qsa("thead th", table);
            const colIndex = headers.findIndex(th => th.textContent.trim().toLowerCase() === columnName.toLowerCase());
            if (colIndex === -1) return;

            headers[colIndex].style.display = isChecked ? "" : "none";

            qsa("tbody tr", table).forEach(row => {
                const cell = row.children[colIndex];
                if (cell) {
                    cell.style.display = isChecked ? "" : "none";
                }
            });
            
            const prefs = JSON.parse(storage.get("erp.column_visibility", "{}"));
            prefs[columnName] = isChecked;
            storage.set("erp.column_visibility", JSON.stringify(prefs));

            updateExportUrl();
        });

        applyColumnVisibility();
    }

    function applyColumnVisibility() {
        const table = qs(".erp-table");
        if (!table) return;

        const prefs = JSON.parse(storage.get("erp.column_visibility", "{}"));
        const headers = qsa("thead th", table);

        headers.forEach((th, index) => {
            const columnName = th.textContent.trim();
            const toggle = qs(`[data-column-toggle="${columnName}"]`);
            if (toggle) {
                const isVisible = prefs[columnName] !== undefined ? prefs[columnName] : toggle.checked;
                toggle.checked = isVisible;

                th.style.display = isVisible ? "" : "none";
                qsa("tbody tr", table).forEach(row => {
                    const cell = row.children[index];
                    if (cell) {
                        cell.style.display = isVisible ? "" : "none";
                    }
                });
            }
        });
    }

    function initGlobalLoaders() {
        // NOTE: alignhr-core.js (body-level submit listener) owns all button loading state.
        // This function only handles the table pagination HTMX interceptor.

        // ═══ TABLE SHELL PAGINATION HTMX INTERCEPTOR ═══
        document.addEventListener("click", function (e) {
            const pageLink = e.target.closest(".table-shell .page-link");
            if (pageLink && pageLink.tagName.toLowerCase() === "a") {
                const tableShell = pageLink.closest(".table-shell");
                if (tableShell && tableShell.id) {
                    const href = pageLink.getAttribute("href");
                    if (href && href !== "#" && !href.startsWith("javascript:")) {
                        e.preventDefault();

                        // Only block if THIS table shell already has an active page-link loader
                        if (tableShell.querySelector(".page-link.is-loading")) return;

                        pageLink.classList.add("is-loading");

                        // Pass source so htmx-request lands on pageLink, not document.body.
                        // Without source, HTMX defaults to body and flashes every
                        // .htmx-indicator on the page simultaneously.
                        htmx.ajax("GET", href, {
                            source: pageLink,
                            target: "#" + tableShell.id,
                            select: "#" + tableShell.id,
                            swap: "outerHTML",
                            pushUrl: true
                        });
                    }
                }
            }
        });
    }

    document.addEventListener("DOMContentLoaded", () => {
        initTheme();
        initSidebar();
        initGlobalLoaders();
        initPermissions();
        initToasts();
        initCommandPalette();
        initValidation();
        initAutosave();
        initWizard();
        initCrudArchitecture();
        initTables();
        initConfirmations();
        initAlignHrCompatibility();
        initCharts();
        initColumnVisibility();
        updateExportUrl();

        document.addEventListener("input", (event) => {
            if (event.target.closest("[name='search']")) {
                updateExportUrl();
            }
        });

        document.addEventListener("change", (event) => {
            if (event.target.closest("[name='departmentId'], [name='subDepartmentId'], [name='gradeId'], [name='divisionId'], [name='status']")) {
                updateExportUrl();
            }
        });

        document.addEventListener("htmx:afterSwap", () => {
            applyColumnVisibility();
            updateExportUrl();
        });

        document.addEventListener("click", (event) => {
            const exportBtn = event.target.closest("#exportBtn");
            if (!exportBtn) return;

            event.preventDefault();

            const url = new URL(exportBtn.href, window.location.origin);
            window.open(url.toString(), "_blank");

            // alignhr-core.js acquires the loading lock on this click (body handler fires first);
            // release it immediately since the download goes to a new tab, not this page.
            window.AlignHR?.Loading.release(exportBtn);
        });
    });
})();
