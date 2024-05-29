VAR enoughMoney = true

#hideportrait:_
\* The air in this office feels stuffy.
\* Drink a cup of water?
    * [Yes]
        #portrait:Tomato_thinking
        Hmm. It's lukewarm.
        
        #hideportrait:_
        #setplayerhp:100@percent
        \* You are hydrated to full health!
        
    * [No]
        