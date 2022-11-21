$(document).ready(() => {

    $("#listaRegiao").select2({ theme: "bootstrap", language: "pt-BR" });
    $("#listaRegiaoPesquisa").select2({ theme: "bootstrap", language: "pt-BR" });

    exibirMensagem(nTipo, sMsg);

});

function enviarPesquisaLiga() {

    const nome = $("#NomeLiga").val();
    const ano = parseInt( $("#AnoTemporada").val() );
    const pais = $("#listaRegiaoPesquisa").val();
    const url = "/liga/";

    var blnValida = (nome.trim() !== "");

    if (!blnValida) {
        blnValida = true;
        blnValida &= (ano > 0);
        blnValida &= (pais.trim() !== "");
    }

    if (blnValida) {

        $("#pesquisa_liga_valida").addClass("invisible");

        // Pesquisando
        $("#lista_liga").empty();
        var tr = $("<tr>");
        tr.append('<td colspan="4" class="pesquisando">pesquisando...</td>');
        tr.append("</tr>");
        $("#lista_liga").append(tr);

        if (nome.trim() !== "") {

            $.getJSON(url + "pesquisarLiga?nome=" + nome, function (retLiga) {

                $("#lista_liga").removeClass("pesquisando");
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

        } else {

            const urlTemporada = url + "pesquisarLigaTemporada?ano=" + ano.toString() + "&pais=" + pais;

            $.getJSON(urlTemporada, function (retLiga) {

                $("#lista_liga").removeClass("pesquisando");
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
    $("#AnoTemporada").val("");
    $("#listaRegiaoPesquisa").val("");
    $("#lista_liga").empty();
}

function informarLiga(id) {
    event.preventDefault();
    $("#liga_IDAPI").val(id);
}