const token = localStorage.getItem('token');

// Formata preços para R$ 0,00
function formatarPreco(valor) {
  return `R$ ${valor.toFixed(2).replace('.', ',')}`;
}

// Atualiza o total geral do carrinho
function atualizarTotalGeral(total) {
  document.getElementById('totalfinal').textContent = formatarPreco(total);
}

// Altera a quantidade de um item
async function alterarQtd(idProduto, delta) {
  try {
    const response = await fetch('/api/Cliente/adicionar-carrinho', {
      method: 'PUT',
      headers: { 
        'Content-Type': 'application/json',
        'Authorization': `Bearer ${token}`
      },
      body: JSON.stringify({ idProduto, delta })
    });

    if (response.ok) {
      // Atualiza apenas os dados do carrinho
      await carregarCarrinho();
    }
  } catch (error) {
    console.error("Erro:", error);
  }
}

// Remove um item (sem recarregar a página)
async function removerItem(idProduto) {
  try {
    const response = await fetch(`/api/Cliente/adicionar-carrinho`, {
      method: 'DELETE',
      headers: { 'Authorization': `Bearer ${token}` }
    });

    if (response.ok) {
      // Atualiza o carrinho após remoção
      await carregarCarrinho();
    }
  } catch (error) {
    console.error("Erro:", error);
  }
}

// Carrega os dados do carrinho
async function carregarCarrinho() {
  const tbody = document.querySelector('tbody');
  const totalFinal = document.getElementById('totalfinal');

  try {
    const response = await fetch('/api/Cliente/ver-carrinho', {
      headers: { 'Authorization': `Bearer ${token}` }
    });

    if (response.ok) {
      const data = await response.json();
      tbody.innerHTML = '';
      let total = 0;

      data.Itens.forEach(item => {
        const subtotal = item.PrecoUnitario * item.Quantidade;
        total += subtotal;

        const tr = document.createElement('tr');
        tr.innerHTML = `
          <td>
            <div class="product">
              <img src="${item.ImageUrl}" alt="${item.Flor}"/>
              <div class="info">
                <div class="name">${item.Flor}</div>
              </div>
            </div>
          </td>
          <td>${formatarPreco(item.PrecoUnitario)}</td>
          <td>
            <div class="qty">
              <button onclick="alterarQtd(${item.Id}, -1)">-</button>
              <span class="quantity">${item.Quantidade}</span>
              <button onclick="alterarQtd(${item.Id}, 1)">+</button>
            </div>
          </td>
          <td>${formatarPreco(subtotal)}</td>
          <td>
            <button class="remove" onclick="removerItem(${item.Id})">
              <i class="bx bx-x"></i>
            </button>
          </td>
        `;
        tbody.appendChild(tr);
      });

      atualizarTotalGeral(total);
    } else {
      alert('Erro ao carregar carrinho');
    }
  } catch (error) {
    console.error("Erro:", error);
  }
}
document.addEventListener('DOMContentLoaded', carregarCarrinho);


// Parte que vai mostrar oq q tá dando erro nessa b
async function carregarCarrinho() {
  const token = localStorage.getItem('token');
  // Redireciona pra página de login se não estiver logado
  if (!token) {
    alert("Faça login para acessar seu carrinho");
    window.location.href = "login_usuario.html";
    return;
  }

  try {
    const response = await fetch('/api/Cliente/ver-carrinho', {
      headers: { 'Authorization': `Bearer ${token}` }
    });

    if (!response.ok) {
      const errorData = await response.json();
      
      if (response.status === 401) {
        alert("Sessão expirada! Faça login novamente.");
        localStorage.removeItem('token');
        window.location.reload();
        return;
      }
      
      throw new Error(errorData.message || "Erro na API");
    }

    const data = await response.json();
    
    // Valida estrutura da resposta
    if (!data?.Itens || !Array.isArray(data.Itens)) {
      throw new Error("Formato de resposta inválido");
    }
    
  } catch (error) {
    console.error("Erro detalhado:", error);
    alert(`Falha ao carregar carrinho: ${error.message}`);
  }

}