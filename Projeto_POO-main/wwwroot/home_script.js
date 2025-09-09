// home_script.js
document.addEventListener('DOMContentLoaded', function() {
  const token = localStorage.getItem('token');
  const logoutBtn = document.getElementById('logout-btn');
  
  // Sempre remova o atributo ao iniciar
  document.body.removeAttribute('data-role');
  
  if (token) {
    logoutBtn.style.display = 'flex';
    
    try {
      // Decodificar o token
      const payload = JSON.parse(atob(token.split('.')[1]));
      
      // Obter a role correta (compatível com diferentes formatos)
      const role = payload.role || 
                  payload['http://schemas.microsoft.com/ws/2008/06/identity/claims/role'];
      
      // Definir o atributo data-role se for produtor
      if (role && role.toLowerCase() === 'produtor') {
        document.body.setAttribute('data-role', 'produtor');
        console.log("Usuário é produtor, atributo definido");
      } else {
        console.log("Usuário não é produtor, role:", role);
      }
    } catch (e) {
      console.error("Erro ao decodificar token:", e);
    }
    
    logoutBtn.addEventListener('click', function() {
      localStorage.removeItem('token');
      document.body.removeAttribute('data-role');
      alert('Você foi desconectado!');
      window.location.href = 'login_usuario.html';
    });
  } else {
    logoutBtn.style.display = 'none';
    console.log("Usuário não logado");
  }
});

console.log("Payload do token:", payload);
console.log("Role encontrada:", role);