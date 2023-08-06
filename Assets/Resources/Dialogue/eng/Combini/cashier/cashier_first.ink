VAR totalPrice = 0
VAR hasMember = false

Hello! #checkparty:BabyCat #portrait:Cashier_neutral
{
    -hasMember:
    Um....<br>Sorry, no pets allowed.
}
-> Greet

=== Greet ===
What can I help you? #viewshop:_ #movechoicebox:_
-> main

=== main ===
    + [ConfirmPurchase]
        Alright. #calculateshop:_
        That will be total {totalPrice} coins.<br>Are you okay with that?
        
            + + [Yes]
                Thank you for your purchase. #portrait:Cashier_happy #payshop:_ #continueshopping:_
                -> Ask
            
            + + [No]
                Feel free to take your time. #continueshopping:_
                -> Greet
                
    + [ExitShop]
        Please come again! #resetchoicebox:_
        -> DONE

=== Ask ===
Is there anything else? #viewshop:_ #movechoicebox:_
-> main