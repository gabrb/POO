// Função para validar email
function validarEmail(email) {
  const re = /^[^\s@]+@[^\s@]+\.[^\s@]+$/;
  return re.test(email);
}

// Função de verificação de login
function usuarioEstaLogado() {
  const token = localStorage.getItem('token');
  if (!token) return false;

  try {
    const payload = JSON.parse(atob(token.split('.')[1]));
    return payload.exp * 1000 > Date.now();
  } catch (e) {
    console.error("Token inválido:", e);
    return false;
  }
}

document.addEventListener("DOMContentLoaded", function () {
  // Redireciona se já estiver logado
  if (usuarioEstaLogado()) {
    window.location.href = 'home.html';
    return;
  }

  // Elementos do cadastro
  const tipoSelect    = document.getElementById("signup-user-type");
  const camposProdutor = document.getElementById("campos-produtor");
  const camposCliente  = document.getElementById("campos-cliente");

  // Marca todos os inputs/selects de uma seção como disabled ou enabled
  function setDisabledEm(secao, disabled) {
    secao.querySelectorAll('input, select, textarea').forEach(c => {
      c.disabled = disabled;
    });
  }

  // Função que mostra/oculta e habilita/desabilita campos
  function toggleCampos() {
    const isProdutor = tipoSelect.value === "produtor";
    const isCliente  = tipoSelect.value === "cliente";

    // Exibição
    camposProdutor.style.display = isProdutor ? "block" : "none";
    camposCliente.style.display  = isCliente  ? "block" : "none";

    // Habilita apenas a seção visível
    setDisabledEm(camposProdutor, !isProdutor);
    setDisabledEm(camposCliente,  !isCliente);
  }

  // Inicializa e escuta mudança
  tipoSelect.addEventListener("change", toggleCampos);
  toggleCampos();
});

// ----- LOGIN -----
// ----- LOGIN -----
document.querySelector('.sign-in-htm')
  .addEventListener('submit', async (e) => {
    e.preventDefault();

    const email = document.getElementById('login-username').value.trim();
    const senha = document.getElementById('login-password').value.trim();

    if (!email || !senha) {
      alert('Preencha todos os campos');
      return;
    }
    if (!validarEmail(email)) {
      alert('Email inválido');
      return;
    }

    try {
      const response = await fetch('http://localhost:5211/api/Auth/login', {
        method: 'POST',
        headers: { 'Content-Type': 'application/json' },
        body: JSON.stringify({ 
          Email: email, 
          Senha: senha })
      });

      if (response.ok) {
        const { token } = await response.json();
        localStorage.setItem('token', token);
        window.location.href = 'home.html';
      } else {
        const err = await response.json();
        alert(`Erro: ${err.message || 'Falha no login'}`);
      }
    } catch {
      alert('Erro de conexão');
    }
  });

// ----- CADASTRO -----
document.getElementById('cadastro-form')
  .addEventListener('submit', async (e) => {
    e.preventDefault();

    const tipo = document.getElementById('signup-user-type').value;
    let dados = {}, url;

    if (tipo === 'cliente') {
      dados = {
        nome:     document.getElementById('signup-nome-cliente').value.trim(),
        telefone: document.getElementById('signup-telefone-cliente').value.trim(),
        endereco: document.getElementById('signup-endereco-cliente').value.trim(),
        gostos:   document.getElementById('signup-gostos').value.trim(),
        email:    document.getElementById('signup-email-cliente').value.trim(),
        senha:    document.getElementById('signup-password-cliente').value.trim()
      };
      url = 'http://localhost:5211/api/Auth/registro-cliente';

    } else if (tipo === 'produtor') {
      dados = {
        nome:      document.getElementById('signup-nome-produtor').value.trim(),
        telefone:  document.getElementById('signup-telefone-produtor').value.trim(),
        endereco:  document.getElementById('signup-endereco-loja').value.trim(),
        nomeLoja:  document.getElementById('signup-nome-loja').value.trim(),
        descricao: document.getElementById('signup-descricao').value.trim(),
        email:     document.getElementById('signup-email-produtor').value.trim(),
        senha:     document.getElementById('signup-password-produtor').value.trim()
      };
      url = 'http://localhost:5211/api/Auth/registro-produtor';

    } else {
      alert('Selecione um tipo de usuário');
      return;
    }

    // Validações básicas
    if (!dados.email || !dados.senha) {
      alert('Email e senha são obrigatórios');
      return;
    }
    if (!validarEmail(dados.email)) {
      alert('Email inválido');
      return;
    }

    try {
      const response = await fetch(url, {
        method: 'POST',
        headers: { 'Content-Type': 'application/json' },
        body: JSON.stringify(dados)
      });

      if (response.ok) {
        alert('Cadastro realizado com sucesso!');
        e.target.reset();
        document.getElementById('tab-1').click(); // volta para login
      } else {
        const err = await response.json();
        alert(`Erro: ${err.message || 'Falha no cadastro'}`);
      }
    } catch {
      alert('Erro de conexão com o servidor');
    }

// Esconde o botão dá página de produtor se não estiver logado como tal
if (usuarioEstaLogado() && usuarioEhProdutor()) {
    addProductBtn.classList.remove('hidden-element');
} else {
    addProductBtn.classList.add('hidden-element');
}

  });