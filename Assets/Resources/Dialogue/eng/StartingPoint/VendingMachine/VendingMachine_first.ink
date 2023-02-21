VAR enoughMoney = true
#hideportrait:_
\* Stack of chilly sodas are aligned neatly within the glass.
\* Buy one soda? (10 coins) #checkplayermoney:10
    * [Yes]
        {
        - enoughMoney:
         #purchaseone:Soda-10
        \* One soda popped out from the machine.
        - else:
        \* Not enough money.
        }
        
    * [No]
        