$(document).ready(function () {

    // Data Table 
    var arrOpcao = {
        "ordering": false,
        "pagingType": "numbers",
        "language": {
            "url": "//cdn.datatables.net/plug-ins/1.10.22/i18n/Portuguese-Brasil.json"
        }
    };

    $('#TimeTabela').DataTable(arrOpcao);
    $('#CampeonatoTabela').DataTable(arrOpcao);
    $('#MercadoTabela').DataTable(arrOpcao);

});