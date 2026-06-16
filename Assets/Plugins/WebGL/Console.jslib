mergeInto(LibraryManager.library, {
    BrowserLog: function(ptr) {
        console.log(UTF8ToString(ptr));
    }
});