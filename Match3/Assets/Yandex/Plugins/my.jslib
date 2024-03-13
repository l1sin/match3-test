mergeInto(LibraryManager.library, {

  GetLanguage: function () {
    var lang = ysdk.environment.i18n.lang;

    var bufferSize = lengthBytesUTF8(lang) + 1;
    var buffer = _malloc(bufferSize);
    stringToUTF8(lang, buffer, bufferSize);
    return buffer;
  },

  GetDomen: function () {
    var domen = ysdk.environment.i18n.tld;

    var bufferSize = lengthBytesUTF8(domen) + 1;
    var buffer = _malloc(bufferSize);
    stringToUTF8(domen, buffer, bufferSize);
    return buffer;
  },

  WatchAdCoins: function () {
    ysdk.adv.showRewardedVideo({
      callbacks: {
        onOpen: () => {
          console.log('Video ad open.');
        },
        onRewarded: () => {
          myGameInstance.SendMessage('WinScreen', 'DoubleCoin');
          console.log('Rewarded double coins');
        },
        onClose: () => {
          console.log('Video ad closed.');
        }, 
        onError: (e) => {
          console.log('Error while open video ad:', e);
        }
      }
    })
  },

  WatchAdExp: function () {
    ysdk.adv.showRewardedVideo({
      callbacks: {
        onOpen: () => {
          console.log('Video ad open.');
        },
        onRewarded: () => {
          myGameInstance.SendMessage('WinScreen', 'DoubleExp');
          console.log('Rewarded double exp');
        },
        onClose: () => {
          console.log('Video ad closed.');
        }, 
        onError: (e) => {
          console.log('Error while open video ad:', e);
        }
      }
    })
  },

  SaveExtern: function (data) {
    if (player.getMode() === 'lite')
    {
      console.log("Saving. No auth. No cloud save, only local.");
    }
    else
    {
      var dataString = UTF8ToString(data);
      var myobj = JSON.parse(dataString);
      player.setData(myobj, true);
      console.log("Saving. Auth. Save to cloud.");
    }
  },

  LoadExtern: function () {
    if (player.getMode() === 'lite')
    {
      console.log("Loading. No auth. Use local save.");
      myGameInstance.SendMessage('---DATA---', 'LoadDataLocal');
    }
    else
    {
      player.getData().then(_data => {
        console.log("Loading. Auth. Use cloud save.");
        const myJSON = JSON.stringify(_data);
        myGameInstance.SendMessage('---DATA---', 'LoadDataCloud', myJSON);
      });
    }
  },

  FullScreenAd: function () {
    ysdk.adv.showFullscreenAdv({
      callbacks: {
        onClose: function(wasShown) {
          // some action after close
        },
        onError: function(error) {
          // some action on error
        }
      }
    })
  },

  GetPrice: function (id) {
    var item = gameShop[id]
    var price = item.priceValue;
    var bufferSize = lengthBytesUTF8(price) + 1;
    var buffer = _malloc(bufferSize);
    stringToUTF8(price, buffer, bufferSize);
    return buffer;
  },

  GameReady: function () {
    ready();
  },

  ReachGoal: function (goal) {
    var goalString = UTF8ToString(goal);
    ym(96554425,'reachGoal',goalString);
  },

  BuyPurchase: function (purchaseID, purchaseIndex) {
    var pIDstring = UTF8ToString(purchaseID);
    payments.purchase({ id: pIDstring }).then(purchase => {
      myGameInstance.SendMessage('MenuController', 'EnablePurchase', purchaseIndex);
    }).catch(err => {
        // Покупка не удалась: в консоли разработчика не добавлен товар с таким id,
        // пользователь не авторизовался, передумал и закрыл окно оплаты,
        // истекло отведенное на покупку время, не хватило денег и т. д.
    })
  },

  CheckPurchase: function (purchaseID, purchaseIndex) {
    var pIDstring = UTF8ToString(purchaseID);
    payments.getPurchases().then(purchases => {
      if (purchases.some(purchase => purchase.productID === pIDstring)) {
        myGameInstance.SendMessage('MenuController', 'EnablePurchase', purchaseIndex);
        console.log('found ' + pIDstring);
      }
    }).catch(err => {
        // Выбрасывает исключение USER_NOT_AUTHORIZED для неавторизованных пользователей.
      console.log('check purcase error');
    })
  },

  ConsumePurchase: function (purchaseToken) {
    var tokenString = UTF8ToString(purchaseToken);
    payments.consumePurchase(tokenString);
    console.log('Purchase consumed');
  },

  FindAllPurchases: function () {
    payments.getPurchases().then(purchases => purchases.forEach((purchase) =>{
      var info = purchase.productID + ',' + purchase.purchaseToken;
      console.log(info);
      myGameInstance.SendMessage('MenuController', 'Consume', info); 
    }));
  },
});