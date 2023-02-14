VAR totalPrice = 0
-> main

=== main ===
#viewshop:_ #portrait:Cashier_neutral #movechoicebox:_
무엇을 도와드릴까요?

    + [구매진행]
        계산 도와드리겠습니다. #calculateshop:_
        총 {totalPrice} 코인이 되겠습니다.<br>구매를 진행하시겠습니까?
        
            + + [예]
                구매해주셔서 감사합니다! #portrait:Cashier_happy #payshop:_ #continueshopping:_
                -> main
            
            + + [아니요]
                천천히 둘러보세요. #continueshopping:_
                -> main
                
    + [상점 나가기]
        감사합니다. #resetchoicebox:_
        -> DONE