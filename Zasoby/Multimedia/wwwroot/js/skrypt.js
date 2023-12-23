document.getElementById('btnAdd').addEventListener('click', function () {
    moveSelectedOptions('#allUsers', '#selectedUsers');
    updateHiddenInput();
});

document.getElementById('btnRemove').addEventListener('click', function () {
    moveSelectedOptions('#selectedUsers', '#allUsers');
    updateHiddenInput();
});

function moveSelectedOptions(from, to) {
    var selectedOptions = document.querySelectorAll(from + ' option:checked');
    var toList = document.querySelector(to);
    Array.from(selectedOptions).forEach(function (option) {
        toList.appendChild(option);
    });
}

function updateHiddenInput() {
    var selectedOptions = document.querySelectorAll('#selectedUsers option');
    var selectedIds = Array.from(selectedOptions).map(option => option.value);
    document.querySelector('input[asp-for="SelectedUserIds"]').value = selectedIds.join(',');
}
