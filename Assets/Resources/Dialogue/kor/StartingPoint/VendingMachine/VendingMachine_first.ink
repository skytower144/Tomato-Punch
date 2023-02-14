VAR enoughMoney = true

꽁꽁 차가워 보이는 음료수들이 유리 너머 깔끔하게 진열되어 있다.
소다 한 캔을 구매할까? (10 코인) #checkplayermoney:10
    * [예]
        {
        - enoughMoney:
         #purchaseone:Soda-10
        소다 한 캔이 자판기로부터 덜컹 튀어나왔다.
        - else:
        그러고 보니 돈이 부족하다.
        }
        
    * [아니오]
        