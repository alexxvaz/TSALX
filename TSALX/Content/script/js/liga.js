$(document).ready(() => {

    $("#listaRegiao").select2({ theme: "bootstrap", language: "pt-BR" });
    $("#listaRegiaoPesquisa").select2({ theme: "bootstrap", language: "pt-BR" });

    exibirMensagem(nTipo, sMsg);

});

function enviarPesquisaLiga() {

    const nome = $("#NomeLiga").val();
    const url = "/liga/";

    var blnValida = false;

    blnValida |= (nome.trim() !== "");

    if (blnValida) {
        $("#pesquisa_liga_valida").addClass("invisible");

        if (nome.trim() !== "") {

            $.getJSON(url + "pesquisarLiga?nome=" + nome, function (retLiga) {

                var oResultado = $("#lista_liga");
                $(oResultado).empty();

                if (retLiga.length > 0) {

                    for (var intI = 0; intI <= retLiga.length - 1; intI++) {

                        var tr = $("<tr>");
                        var sNomePais = "";

                        if (retLiga[intI].NomePais.trim() != "null") {
                            sNomePais = retLiga[intI].NomePais;
                        }

                        tr.append('<td><img src="' + retLiga[intI].Escudo + '" class="logo"/></td>');
                        tr.append('<td>' + retLiga[intI].Nome + '</td>');
                        tr.append('<td>' + sNomePais + '</td>');
               
                        tr.append('<td class="text-center"><a href="#" class="btn btn-outline-secondary" data-dismiss="modal" onclick="informarLiga(' + retLiga[intI].ID + ')"><i class="fa fa-info-circle" aria-hidden="true"></i></a></td>');
                        tr.append('</tr>');

                        $(oResultado).append(tr);

                    }
                } else {

                    var tr = $("<tr>");
                    tr.append('<td colspan="4" class="sem-registros">Não há registros selecionados</td>');
                    tr.append("</tr>");
                    $(oResultado).append(tr);
                }


            });
        }
    }
    else {
        $("#pesquisa_liga_valida").removeClass("invisible");
    }

}

function limparPesquisa() {

    $("#NomeLiga").val("");
}

function informarLiga(id) {
    event.preventDefault();
    $("#liga_IDAPI").val(id);
}