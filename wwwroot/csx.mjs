export function init(iframe) {
    const url = new URL(import.meta.url);
    iframe.onload = () => iframe.contentWindow.postMessage({ init: iframe.lastChild.data }, url.origin);
    url.pathname = url.pathname.replace(/\.mjs$/, '.html')
    iframe.src = url.href;
}