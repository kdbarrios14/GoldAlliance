function hideOnLoad() {
    $('#ToAccount').hide();
}

$(document).ready(function () {
    hideOnLoad();
    $('#TransactionTypes').change(function () {
        var value = $('#TransactionTypes option:selected').text();
        if (value == "Transfer") {
            $('#ToAccount').show();
        }
        else {
            hideOnLoad();
        }
    });
})