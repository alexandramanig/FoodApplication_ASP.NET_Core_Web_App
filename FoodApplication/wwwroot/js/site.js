// URL și cheie API
let apiURL = "https://forkify-api.herokuapp.com/api/v2/recipes";
let apikey = "7ecac8c2-fcdf-42cf-bfca-0d9f6d9cf4b1";

// Obține rețetele
async function GetRecipes(recipeName, id, isAllShow) {
    try {
        let resp = await fetch(`${apiURL}?search=${recipeName}&key=${apikey}`);
        if (!resp.ok) throw new Error(`HTTP error! status: ${resp.status}`);
        let result = await resp.json();
        let Recipes = isAllShow ? result.data.recipes : result.data.recipes.slice(0, 6);
        showRecipe(Recipes, id);
    } catch (error) {
        console.error("Error fetching recipes:", error);
    }
}

// Afișează rețetele
function showRecipe(recipes, id) {
    if (!recipes || recipes.length === 0) {
        $('#' + id).html("<p>No recipes found.</p>");
        return;
    }
    $.ajax({
        contentType: "application/json; charset=utf-8",
        dataType: 'html',
        type: 'POST',
        url: '/Recipe/GetRecipeCard',
        data: JSON.stringify(recipes),
        success: function (htmlResult) {
            $('#' + id).html(htmlResult);
        },
        error: function (err) {
            console.error("Error fetching recipe cards:", err);
        }
    });
}

// Obține detalii despre rețeta pentru comandă
async function getOrderRecipe(id, showId) {
    try {
        let resp = await fetch(`${apiURL}/${id}?key=${apikey}`);
        if (!resp.ok) throw new Error(`HTTP error! status: ${resp.status}`);
        let result = await resp.json();
        let recipe = result.data.recipe;
        showOrderRecipeDetails(recipe, showId);
    } catch (error) {
        console.error("Error fetching order recipe:", error);
    }
}

// Afișează detaliile despre comandă
function showOrderRecipeDetails(orderRecipeDetails, showId) {
    $.ajax({
        url: '/Recipe/ShowOrder',
        data: orderRecipeDetails,
        dataType: 'html',
        type: 'POST',
        success: function (htmlResult) {
            $('#' + showId).html(htmlResult);
        },
        error: function (err) {
            console.error("Error fetching order recipe details:", err);
        }
    });
}

// Actualizează cantitatea
function quantity(option) {
    let qtyElement = $('#qty');
    let priceElement = $('#price');
    let totalAmountElement = $('#totalAmount');

    if (!qtyElement.length || !priceElement.length || !totalAmountElement.length) {
        console.error("Required elements not found in the DOM.");
        return;
    }

    let qty = parseInt(qtyElement.val());
    let price = parseInt(priceElement.val());
    let totalAmount = 0;

    if (option === 'inc') {
        qty += 1;
    } else {
        qty = qty > 1 ? qty - 1 : 1;
    }

    totalAmount = price * qty;
    qtyElement.val(qty);
    totalAmountElement.val(totalAmount);
}

// Adaugă la coș
async function cart() {
    let iTag = $(this).children('i')[0];
    let recipeId = $(iTag).attr('data-recipeId');
    console.log(recipeId);

    if ($(iTag).hasClass('fa-regular')) {
        try {
            let resp = await fetch(`${apiURL}/${recipeId}?key=${apikey}`);
            if (!resp.ok) throw new Error(`HTTP error! status: ${resp.status}`);
            let result = await resp.json();
            let cart = result.data.recipe;
            cart.RecipeId = recipeId;
            delete cart.id;
            cartRequest(cart, 'SaveCart');
        } catch (error) {
            console.error("Error adding to cart:", error);
        }
    } else {
        console.log("Item is already in the cart.");
    }
}

// Trimitere cerere pentru coș
function cartRequest(data, action) {
    $.ajax({
        url: '/Cart/' + action,
        type: 'POST',
        data: data,
        success: function (resp) {
            console.log("Cart updated successfully:", resp);
        },
        error: function (err) {
            console.error("Error updating cart:", err);
        }
    });
}
