VAR totalPrice = 0
VAR hasMember = false

-> main

=== main ===
Hello! #checkparty:BabyCat #viewshop:_ #portrait:Cashier_neutral #movechoicebox:_
{
    -hasMember:
    Um....<br>Sorry, no pets allowed.
}
What can I help you?

    + [ConfirmPurchase]
        Alright. #calculateshop:_
        That will be total {totalPrice} coins.<br>Are you okay with that?
        
            + + [Yes]
                Thank you for your purchase. #portrait:Cashier_happy #payshop:_ #continueshopping:_
                -> main
            
            + + [No]
                Feel free to take your time. #continueshopping:_
                -> main
                
    + [ExitShop]
        Please come again! #resetchoicebox:_
        -> DONE