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
function abrirCriar() {
    fetch('/JAX/Adicionar')
        .then(response => response.text())
        .then(html => {
            document.getElementById('formCriar').innerHTML = html;
            new bootstrap.Modal(document.getElementById('modalCriar')).show();
        });
}
function salvarCriar() {
    const form = document.getElementById('formCriar');
    const formData = new FormData(form);

    fetch('/JAX/Adicionar', {
        method: 'POST',
        body: formData
    })
        .then(r => r.json())
        .then(data => {
            if (data.success) {
                location.reload(); 
            } else {
                alert("Erro ao criar");
            }
        });
}
function abrirEditar(id) {
    fetch(`/JAX/Editar/${id}`)
        .then(response => response.text())
        .then(html => {
            document.getElementById('formEditar').innerHTML = html;
            new bootstrap.Modal(document.getElementById('modalEditar')).show();
        });
}

function salvarEdicao() {
    const form = document.getElementById('formEditar');
    const formData = new FormData(form);

    fetch('/JAX/Editar', {
        method: 'POST',
        body: formData
    })
        .then(r => r.json())
        .then(data => {
            if (data.success) {
                location.reload(); 
            } else {
                alert("Erro ao editar");
            }
        });
}
function Excluir(id) {
    fetch(`/JAX/Excluir/${id}`)
        .then(response => response.text())
        .then(html => {
            document.getElementById('formExcluir').innerHTML = html;
            new bootstrap.Modal(document.getElementById('modalExcluir')).show();
        });
}
function ExcluirPessoa(id) {
    fetch(`/JAX/ExcluirPessoa/${id}`, {
            method: 'POST'
        })
            .then(r => r.json())
            .then(data => {
                if (data.success) {
                    location.reload();
                } else {
                    alert("Erro ao excluir");
                }
            });
} 