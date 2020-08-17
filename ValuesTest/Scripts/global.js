
function updateAddToCartCount() {
    var cookie = $.cookie("CartProducts");

    if (cookie === undefined)
        $("#cartCount").text("0");
    else
        $("#cartCount").text(`${$.cookie("CartProducts").split("-").length}`);
}

updateAddToCartCount();

