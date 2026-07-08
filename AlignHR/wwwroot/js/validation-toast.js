$(function () {

    // ── 1. Map jQuery Validate → Bootstrap classes ──────────────────────────
    $.validator.setDefaults({
        highlight: function (element) {
            $(element).addClass('is-invalid').removeClass('is-valid');
            markTabErrors();
        },
        unhighlight: function (element) {
            $(element).removeClass('is-invalid').addClass('is-valid');
            markTabErrors();
        },
        errorElement: 'span',
        errorClass: 'invalid-feedback',
        errorPlacement: function (error, element) {
            var existing = element.siblings('.invalid-feedback');
            if (existing.length) {
                existing.text(error.text());
            } else {
                error.insertAfter(element);
            }
        }
    });

    // ── 2. Server-side postback — convert ASP.NET classes → Bootstrap ────────
    $('.input-validation-error').addClass('is-invalid');
    $('.field-validation-error').addClass('invalid-feedback d-block');

    // Mark tabs on initial page load (catches server-side postback errors)
    markTabErrors();

    // ── 3. On invalid submit — switch tab + toast ────────────────────────────
    $('form').each(function () {
        var $form = $(this);

        $form.on('invalid-form.validate', function (event, validator) {
            var errors = validator.numberOfInvalids();
            if (!errors) return;

            markTabErrors();

            // Switch to the tab containing the first invalid field
            if (validator.errorList.length > 0) {
                var $firstField = $(validator.errorList[0].element);
                var $pane = $firstField.closest('.tab-pane');

                if ($pane.length && !$pane.hasClass('active')) {
                    var paneId = $pane.attr('id');
                    var $tabBtn = $('[data-bs-target="#' + paneId + '"]');
                    if ($tabBtn.length && typeof bootstrap !== 'undefined') {
                        bootstrap.Tab.getOrCreateInstance($tabBtn[0]).show();
                        setTimeout(function () {
                            $firstField[0].scrollIntoView({ behavior: 'smooth', block: 'center' });
                            $firstField[0].focus();
                        }, 150);
                    }
                } else {
                    $firstField[0].scrollIntoView({ behavior: 'smooth', block: 'center' });
                    $firstField[0].focus();
                }
            }

            // Toast
            var total   = validator.errorList.length;
            var firstMsg = validator.errorList[0].message;
            if (total > 1) {
                showToast(total + ' validation errors. Please check highlighted tabs.', 'error');
            } else {
                showToast(firstMsg, 'error');
            }
        });
    });

    // ── 4. Clear tab error dots when a field is corrected ───────────────────
    $(document).on('input change', '.is-invalid', function () {
        // Small delay so jQuery Validate has time to re-assess
        setTimeout(markTabErrors, 50);
    });

});

// ── markTabErrors ────────────────────────────────────────────────────────────
// Scans every tab pane for .is-invalid fields and sets .tab-has-error on the
// matching tab button. Clears the class on panes that are now clean.
function markTabErrors() {
    $('.tab-pane').each(function () {
        var paneId  = $(this).attr('id');
        if (!paneId) return;

        var $tabBtn  = $('[data-bs-target="#' + paneId + '"]');
        if (!$tabBtn.length) return;

        var hasError = $(this).find('.is-invalid').length > 0;
        $tabBtn.toggleClass('tab-has-error', hasError);
    });
}
