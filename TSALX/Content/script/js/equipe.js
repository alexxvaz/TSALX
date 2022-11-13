$(document).ready(() => {

    $("#listaRegiao").select2({ theme: "bootstrap", language: "pt-BR" });
    $("#listaRegiaoPesquisa").select2({ theme: "bootstrap", language: "pt-BR" });

    exibirMensagem(nTipo, sMsg);

});

function enviarPesquisaEquipe() {

    const nome = $("#NomeEquipe").val();
    const url = "/equipe/";

    var blnValida = false;
    
    blnValida |= ( nome.trim() !== "" );

    if (blnValida) {
        $("#pesquisa_equipe_valida").addClass("invisible");

        if (nome.trim() !== "") {

            $.getJSON(url + "pesquisarEquipe?nome=" + nome, function (retEquipe) {

                var oResultado = $("#lista_equipe");
                $(oResultado).empty();

                if (retEquipe.length > 0) {

                    for (var intI = 0; intI <= retEquipe.length - 1; intI++) {

                        var tr = $("<tr>");
                        var sNomePais = "";

                        if (retEquipe[intI].NomePais.trim() != "null") {
                            sNomePais = retEquipe[intI].NomePais;
                        }

                        tr.append('<td><img src="' + retEquipe[intI].Escudo + '" class="logo"/></td>');
                        tr.append('<td>' + retEquipe[intI].Nome + '</td>');
                        tr.append('<td>' + sNomePais + '</td>');
                        if (retEquipe[intI].Selecao) {
                            tr.append('<td><i class="fa fa-check-circle-o fa-lg" aria-hidden="true"></i></td>');
                        } else {
                            tr.append('<td></td>');
                        }
                        
                        tr.append('<td class="text-center"><a href="#" class="btn btn-outline-secondary" data-dismiss="modal" onclick="informarEquipe(' + retEquipe[intI].ID + ')"><i class="fa fa-info-circle" aria-hidden="true"></i></a></td>');
                        tr.append('</tr>');

                        $(oResultado).append(tr);

                    }
                } else {

                    var tr = $("<tr>");
                    tr.append('<td colspan="5" class="sem-registros">Não há registros selecionados</td>');
                    tr.append("</tr>");
                    $(oResultado).append(tr);
                }


            });
        }
    }
    else {
        $("#pesquisa_equipe_valida").removeClass("invisible");
    }

}

function limparPesquisa() {

    $("#NomeEquipe").val("");
}

function informarEquipe(id) {
    event.preventDefault();
    $("#equipe_IDAPI").val(id);
}