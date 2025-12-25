window.blazorCulture = {
  set: function (culture) {
    if (!culture) {
      culture = "tr";
    }

    document.cookie = `.AspNetCore.Culture=c=${culture}|uic=${culture}; path=/`;
    location.reload();
  }
};
