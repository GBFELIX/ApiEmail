document.getElementById("checkAll").addEventListener("change", function () {
    var checkboxes = document.querySelectorAll(".checkbox-item");
    checkboxes.forEach(cb => cb.checked = this.checked);
});

function contarSelecionados() {
    return document.querySelectorAll(".checkbox-item:checked").length;
}

function abrirModalEmail() {
    const total = contarSelecionados();
    if (total === 0) {
        alert("Selecione ao menos uma pessoa.");
        return;
    }
    document.getElementById("quantidadeEmail").textContent = total;
    new bootstrap.Modal(document.getElementById("modalEmail")).show();
}

function abrirModalExcel() {
    const total = contarSelecionados();
    if (total === 0) {
        alert("Selecione ao menos uma pessoa.");
        return;
    }
    const formExcel = document.getElementById("formExcel");
    formExcel.innerHTML += "";
    const selecionados = document.querySelectorAll(".checkbox-item:checked");
    selecionados.forEach(item => {
        const input = document.createElement("input");
        input.type = "hidden";
        input.name = "idsSelecionados";
        input.value = item.value;
        formExcel.appendChild(input);
    });

    document.getElementById("quantidadeExcel").textContent = total;
    new bootstrap.Modal(document.getElementById("modalExcel")).show();
}