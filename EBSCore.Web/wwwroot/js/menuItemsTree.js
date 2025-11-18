window.menuItemsTree = (function () {
  function buildContext(dotnet) {
    return {
      create: {
        label: 'Add',
        action: function (obj) {
          const node = $('#MenuItemsTree').jstree(true).get_node(obj.reference);
          dotnet.invokeMethodAsync('OnCreate', node.id);
        }
      },
      edit: {
        label: 'Edit',
        action: function (obj) {
          const node = $('#MenuItemsTree').jstree(true).get_node(obj.reference);
          dotnet.invokeMethodAsync('OnEdit', node.id);
        }
      },
      delete: {
        label: 'Delete',
        action: function (obj) {
          const node = $('#MenuItemsTree').jstree(true).get_node(obj.reference);
          dotnet.invokeMethodAsync('OnDelete', node.id);
        }
      }
    };
  }

  function parseNodes(nodes) {
    if (Array.isArray(nodes)) return nodes;
    try { return JSON.parse(nodes); } catch(e) { return []; }
  }

  function init(elementId, nodes, dotnetRef) {
    const el = $('#' + elementId);
    try { el.jstree('destroy').empty(); } catch (e) {}
    el.jstree({
      core: { data: parseNodes(nodes), check_callback: true },
      plugins: ['contextmenu', 'dnd'],
      contextmenu: {
        items: function (node) {
          // capture node in closures like the MVC page
          return {
            create: {
              label: 'Add',
              action: function () { dotnetRef.invokeMethodAsync('OnCreate', node.id); }
            },
            edit: {
              label: 'Edit',
              action: function () { dotnetRef.invokeMethodAsync('OnEdit', node.id); }
            },
            delete: {
              label: 'Delete',
              action: function () { dotnetRef.invokeMethodAsync('OnDelete', node.id); }
            }
          };
        }
      }
    });
  }

  function update(elementId, nodes, dotnetRef) {
    const el = $('#' + elementId);
    const api = el.jstree(true);
    if (api) {
      api.settings.core.data = parseNodes(nodes);
      api.refresh();
    } else {
      init(elementId, nodes, dotnetRef || { invokeMethodAsync: function () { } });
    }
  }

  return { init, update };
})();
