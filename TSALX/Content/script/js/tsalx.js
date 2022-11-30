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
    $('#MercadoTabela').DataTable(arrOpcao);
    $('#RegiaoTabela').DataTable(arrOpcao);
    $('#LigaTabela').DataTable(arrOpcao);
    $('#TSALXTabela').DataTable(arrOpcao);

});

function exibirMensagem(tipo, mensagem) {

    if (mensagem != "") {

        toastr.options = {
            "closeButton": true,
            "debug": false,
            "newestOnTop": false,
            "progressBar": false,
            "positionClass": "toast-top-full-width",
            "preventDuplicates": true,
            "onclick": null,
            "showDuration": "2000",
            "hideDuration": "1000",
            "timeOut": "5000",
            "extendedTimeOut": "1000",
            "showEasing": "swing",
            "hideEasing": "linear",
            "showMethod": "fadeIn",
            "hideMethod": "fadeOut"
        };

        var sTipo = "error";

        if (tipo == 0) { 
            sTipo = "success";
        } else if (tipo == 1) {   // Processo
            sTipo = "warning";
        }

        toastr[sTipo](mensagem, "Trade Sport ALX");
    }
}