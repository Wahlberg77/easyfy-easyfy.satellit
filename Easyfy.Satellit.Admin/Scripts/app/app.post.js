var satellit = satellit || {};

satellit.search = function () {
  var refreshData = function () {
    
    var searchTerm = $('input[data-action="post-search"]').val();

    var filters = [];
    if (searchTerm.length > 0) {
      filters.push("q=" + searchTerm);
    }

    $.ajax({
      url: "?" + filters.join("&"),
      cache: false,
      success: function (data) {
        $('[data-placeholder="post-list"]').html(data);
        //Något med autocomplete för att kunna söka på bokstäver istället för hela ord?
        //$('[data-placeholder="post-list"]').autocomplete({source: getTitle});
      }
    });
  };
        return {
          refreshData: refreshData
        };
        }();


$(document).on("keyup", 'input[data-action="post-search"]', function (e) {
  satellit.search.refreshData();
  return false;
});

//Datepicker 
$(function () {
  //$("#datepicker").datepicker();
});


