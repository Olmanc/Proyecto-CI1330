$('#btnRight').click(function (e) {
    var seleccionados = $('#recursosAsignados option:selected');
    if (seleccionados.length == 0) {
        alert("no hay recursos seleccionados");
        e.preventDefault();
    }

    $('#recursosDisponibles').append($(seleccionados).clone());
    $(seleccionados).remove();
    e.preventDefault();
});

$('#btnLeft').click(function (e) {
    var seleccionados = $('#recursosDisponibles option:selected');
    if (seleccionados.length == 0) {
        alert("no hay recursos seleccionados");
        e.preventDefault();
    }

    $('#recursosAsignados').append($(seleccionados).clone());
    $(seleccionados).remove();
    e.preventDefault();
});

$('#btnSubmit').click(function (e) {
    $('#recursosAsignados option').prop('selected', true);
    $('#recursosDisponibles option').prop('selected', true);
});