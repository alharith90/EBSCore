window.unitTree = (function () {
  function parseNodes(nodes) {
    if (Array.isArray(nodes)) return nodes;
    try { return JSON.parse(nodes); } catch (e) { return []; }
  }

  function init(elementId, nodes, dotnetRef) {
    const el = $('#' + elementId);
    try { el.jstree('destroy').empty(); } catch (e) { }
    el.jstree({
      core: { data: parseNodes(nodes), check_callback: true }
    });
    el.off('changed.jstree').on('changed.jstree', function (e, data) {
      if (data && data.node) {
        dotnetRef.invokeMethodAsync('OnNodeChanged', data.node.id, data.node.text);
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

  function select(elementId, id) {
    const api = $('#' + elementId).jstree(true);
    if (api && id) {
      api.deselect_all();
      api.select_node(id);
    }
  }

  return { init, update, select };
})();

