'user strict';

const listener = "http://localhost:60024/";

chrome.runtime.onInstalled.addListener(function () {
    chrome.storage.sync.set({ color: '#3aa757' }, function () {
        console.log('The color is green.');
    });

    // chrome.declarativeContent.onPageChanged.removeRules(undefined, function () {
    //     console.log("rule removed");
    //     chrome.declarativeContent.onPageChanged.addRules([{
    //         conditions: [new chrome.declarativeContent.PageStateMatcher({
    //             pageUrl: { hostEquals: 'developer.chrome.com' },
    //         })
    //         ],
    //         actions: [new chrome.declarativeContent.ShowPageAction()]
    //     }], function () {
    //         console.log("rule added");
    //     });
    // });


});

chrome.tabs.onActivated.addListener(function (activeInfo) {
    chrome.tabs.get(activeInfo.tabId, function (tab) {
        notifyLocalApp(tab);
    });
});

chrome.tabs.onUpdated.addListener(function (tabId, changeInfo, tab) {
    if (changeInfo.status === "loading" && tab.url !== "chrome://newtab/") {
        notifyLocalApp(tab);
    }
});

function notifyLocalApp(tab) {
    $.post(listener, {
        url: tab.url,
        title: tab.title
    }, function (result) {
        if (result === "b") {
            chrome.tabs.remove(tab.id);
        }
    });
}