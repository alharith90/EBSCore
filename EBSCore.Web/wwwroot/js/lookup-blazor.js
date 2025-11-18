window.LookupBlazor = (function(){
  function init(options){
    // options: { apiBase, labels }
    $(function(){
      buildEvents(options);
      loadLookups(options);
    });
  }

  function loadLookups(options){
    var getUrl = options.apiBase + '/Get';
    $.get(getUrl, function(res){
      var data = res;
      try {
        if (typeof res === 'string') data = JSON.parse(res);
      } catch(e) { /* keep res */ }
      buildTree(data, options);
    });
  }

  function buildTree(dsLookups, options){
    var $tree = $('#LookupsTree');
    $tree.jstree('destroy').empty();
    $tree.jstree({
      core: { data: dsLookups },
      plugins: ['contextmenu','dnd'],
      contextmenu: {
        items: function(node){
          var menu = {};
          menu.add = { label: options.labels.add, action: function(){ Save(node); } };
          menu.edit = { label: options.labels.edit, action: function(){ Edit(node, options); } };
          menu.delete = { label: options.labels.del, action: function(){ Delete(node, options); } };
          return menu;
        }
      }
    });
  }

  function Save(node){
    $('#ParentID').val(node.id);
    ClearForm();
    $('#modalAddLookup').modal('show');
  }

  function Edit(node, options){
    $('#LookupID').val(node.id);
    $.get(options.apiBase + '/GetOne?LookupID=' + node.id, function(res){
      var lookup = res;
      try { if (typeof res === 'string') { var arr = JSON.parse(res); lookup = arr && arr[0] ? arr[0] : {}; } } catch(e){}
      $('#LookupType').val(lookup.LookupType);
      $('#Level').val(lookup.Level);
      $('#LookupDescriptionAr').val(lookup.LookupDescriptionAr);
      $('#LookupDescriptionEn').val(lookup.LookupDescriptionEn);
      $('#ParentID').val(lookup.ParentID);
      $('#Status').prop('checked', !!lookup.Status);
      $('#modalAddLookup').modal('show');
    });
  }

  function Delete(node, options){
    Swal.fire({
      icon: 'warning',
      title: options.labels.confirmDelete,
      text: options.labels.areYouSure,
      showCancelButton: true,
      confirmButtonText: options.labels.yesDelete,
      cancelButtonText: options.labels.cancel
    }).then(function(result){
      if(result.isConfirmed){
        $.ajax({
          url: options.apiBase + '/Delete',
          method: 'DELETE',
          contentType: 'application/json',
          data: JSON.stringify({ LookupID: node.id })
        }).done(function(){
          loadLookups(options);
          toast('success', options.labels.deleted);
        }).fail(function(){
          Swal.fire({ icon: 'error', title: options.labels.error, text: options.labels.tryAgain });
        });
      }
    });
  }

  function buildEvents(options){
    $('#bOpenModal').on('click', function(){ ClearForm(); });
    $('#frmLookup').on('submit', function(e){
      e.preventDefault();
      var payload = {
        LookupID: $('#LookupID').val(),
        LookupType: $('#LookupType').val(),
        Level: $('#Level').val(),
        LookupDescriptionAr: $('#LookupDescriptionAr').val(),
        LookupDescriptionEn: $('#LookupDescriptionEn').val(),
        ParentID: $('#ParentID').val(),
        Status: $('#Status').is(':checked')
      };
      $.ajax({
        url: options.apiBase + '/Save',
        method: 'POST',
        contentType: 'application/json',
        data: JSON.stringify(payload)
      }).done(function(){
        loadLookups(options);
        ClearForm();
        $('#bCloseModal').click();
        toast('success', options.labels.saved);
      }).fail(function(xhr){
        var msg = (xhr && xhr.responseJSON && xhr.responseJSON.Message) || options.labels.error;
        Swal.fire({ icon: 'error', title: options.labels.error, text: msg });
      });
    });
  }

  function ClearForm(){
    $('#LookupID').val('');
    $('#LookupType').val('');
    $('#Level').val('');
    $('#LookupDescriptionAr').val('');
    $('#LookupDescriptionEn').val('');
    $('#ParentID').val('');
    $('#Status').prop('checked', false);
  }

  function toast(icon, title){
    if(window.Swal){
      const Toast = Swal.mixin({ toast:true, position:'top-end', showConfirmButton:false, timer:3000 });
      Toast.fire({ icon: icon, title: title });
    }
  }

  return { init };
})();
