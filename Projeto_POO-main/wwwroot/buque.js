document.addEventListener("DOMContentLoaded", function() {
    // Elementos do DOM
    const productGrid = document.getElementById('product-grid');
    const filterButtons = document.querySelectorAll('.filter-btn');
    const cartCount = document.getElementById('cart-count');
    
    // Dados dos produtos
    const bouquets = [
        {
            id: 1,
            name: "Buquê de Rosas Vermelhas",
            price: 120.00,
            category: "rosas",
            image: "https://images.unsplash.com/photo-1559563362-c667ba5f5480?ixlib=rb-1.2.1&auto=format&fit=crop&w=500&q=80",
            description: "12 rosas vermelhas frescas com folhagem complementar.",
            rating: 4.8
        },
        {
            id: 2,
            name: "Buquê de Girassóis",
            price: 150.00,
            category: "girassois",
            image: "https://images.unsplash.com/photo-1597848212624-a19eb35e2651?ixlib=rb-1.2.1&auto=format&fit=crop&w=500&q=80",
            description: "5 girassóis vibrantes com folhagem tropical.",
            rating: 4.9
        },
        {
            id: 3,
            name: "Buquê de Orquídeas",
            price: 180.00,
            category: "orquideas",
            image: "https://images.unsplash.com/photo-1518977676601-b53f82aba655?ixlib=rb-1.2.1&auto=format&fit=crop&w=500&q=80",
            description: "Elegante arranjo com orquídeas brancas.",
            rating: 4.7
        },
        {
            id: 4,
            name: "Buquê Romântico",
            price: 135.00,
            category: "mix",
            image: "https://images.unsplash.com/photo-1452827073306-6e6e661baf57?ixlib=rb-1.2.1&auto=format&fit=crop&w=500&q=80",
            description: "Mix de rosas cor-de-rosa e flores do campo.",
            rating: 4.6
        },
        {
            id: 5,
            name: "Buquê Premium",
            price: 220.00,
            category: "mix",
            image: "https://images.unsplash.com/photo-1466692476868-aef1dfb1e735?ixlib=rb-1.2.1&auto=format&fit=crop&w=500&q=80",
            description: "Seleção premium de flores sazonais.",
            rating: 5.0
        },
        {
            id: 6,
            name: "Buquê de Rosas Brancas",
            price: 130.00,
            category: "rosas",
            image: "https://images.unsplash.com/photo-1490750967868-88aa4486c946?ixlib=rb-1.2.1&auto=format&fit=crop&w=500&q=80",
            description: "12 rosas brancas com toque de eucalipto.",
            rating: 4.7
        }
    ];

    // Inicializar carrinho se não existir
    if (!localStorage.getItem('cart')) {
        localStorage.setItem('cart', JSON.stringify([]));
    }

    // Atualizar contador do carrinho
    updateCartCount();

    // Event listeners para filtros
    filterButtons.forEach(button => {
        button.addEventListener('click', () => {
            filterButtons.forEach(btn => btn.classList.remove('active'));
            button.classList.add('active');
            filterProducts(button.dataset.filter);
        });
    });

    // Renderizar todos os produtos inicialmente
    renderProducts(bouquets);

    // Função para renderizar produtos
    function renderProducts(products) {
        productGrid.innerHTML = '';
        
        if (products.length === 0) {
            productGrid.innerHTML = '<p class="no-products">Nenhum buquê encontrado nesta categoria.</p>';
            return;
        }
        
        products.forEach(product => {
            const productCard = document.createElement('div');
            productCard.className = 'product-card';
            productCard.innerHTML = `
                <div class="product-image">
                    <img src="${product.image}" alt="${product.name}" loading="lazy">
                    <div class="product-badge">${product.rating.toFixed(1)} <i class="fas fa-star"></i></div>
                </div>
                <div class="product-info">
                    <h3>${product.name}</h3>
                    <p class="product-description">${product.description}</p>
                    <div class="price">R$ ${product.price.toFixed(2)}</div>
                    <button class="btn add-to-cart" data-id="${product.id}">
                        <i class="fas fa-cart-plus"></i> Adicionar
                    </button>
                </div>
            `;
            productGrid.appendChild(productCard);
        });
        
        // Adicionar event listeners aos botões de adicionar ao carrinho
        document.querySelectorAll('.add-to-cart').forEach(button => {
            button.addEventListener('click', addToCart);
        });
    }

    // Função para filtrar produtos
    function filterProducts(category) {
        if (category === 'all') {
            renderProducts(bouquets);
        } else {
            const filtered = bouquets.filter(bouquet => bouquet.category === category);
            renderProducts(filtered);
        }
    }

    // Função para adicionar ao carrinho
    function addToCart(event) {
        const productId = parseInt(event.target.dataset.id);
        const product = bouquets.find(p => p.id === productId);
        
        let cart = JSON.parse(localStorage.getItem('cart'));
        
        const existingItem = cart.find(item => item.id === productId);
        
        if (existingItem) {
            existingItem.quantity += 1;
        } else {
            cart.push({
                id: product.id,
                name: product.name,
                price: product.price,
                image: product.image,
                quantity: 1
            });
        }
        
        localStorage.setItem('cart', JSON.stringify(cart));
        updateCartCount();
        
        // Feedback visual
        const button = event.target;
        button.innerHTML = '<i class="fas fa-check"></i> Adicionado';
        button.style.backgroundColor = '#4CAF50';
        
        setTimeout(() => {
            button.innerHTML = '<i class="fas fa-cart-plus"></i> Adicionar';
            button.style.backgroundColor = '#2f4f4f';
        }, 1500);
    }

    // Função para atualizar contador do carrinho
    function updateCartCount() {
        const cart = JSON.parse(localStorage.getItem('cart')) || [];
        cartCount.textContent = cart.reduce((total, item) => total + item.quantity, 0);
    }
});