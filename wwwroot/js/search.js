$(document).ready(function () {
    var searchInput = $('#search');
    var searchResultsContainer = $('#search-results-container');

    searchInput.on('input', function () {
        var searchQuery = searchInput.val().trim();

        if (searchQuery.length > 0) {
            $.ajax({
                url: '/Products/Search?search=' + searchQuery,
                dataType: 'json',
                success: function (data) {
                    searchResultsContainer.empty(); // clear previous search results
                    $.each(data, function (index, product) {
                        var productHtml = '<div class="product">' +
                            '<img src="' + product.image + '">' +
                            '<h2>' + product.name + '</h2>' +
                            '</div>';
                        searchResultsContainer.append(productHtml);
                    });
                }
            });
        } else {
            searchResultsContainer.empty(); // clear search results if search query is empty
        }
    });
});