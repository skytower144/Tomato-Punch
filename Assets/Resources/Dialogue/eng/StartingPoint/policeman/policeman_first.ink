VAR isQuestActive = false
VAR isQuestCompleted = false
VAR br = "<br>"

#hideportrait:_
Hmph. 
I'm angry.
Someone definitely stole my donut. #hasquest:FindTheDonut
{
    - isQuestActive:
    Hm? #checkquest:FindTheDonut
    {
        - isQuestCompleted:
        Is it a scrumptious donut that I smell?
        \* You gave the scrumptious donut to the policeman. #removeitem:Donut
        YES! YES! This is the one!{br}What a lifesaver!
        wait a minute...{br}this donut...
        OH MY GOODNESS! #animate:shocked
        #playerdirection:LEFT@1.7
        #animate:fainted
        #changeidle:StartingPoint_Donut@isangry
        #teleport:StartingPoint_Donut@30@41
        #setactive:StartingPoint_Donut@true
        #focusanimate:StartingPoint_Donut@angry
        #battletarget:StartingPoint_Donut
        #continuetalk:_
        #nextdialogue:policeman_fainted
        
        - else:
        What's that crumb on your face?
        
        oops. I mean, um.... #portrait:tomato_neutral
        
        ...... #hideportrait:_
        
        It's long past my snack time,{br}and all I find is some crumbs on a random kid's face.
        Where on earth is my stupid donut?
        It's gotta be Bob. I just know it's those greedy cheeks.
        
        ........? #portrait:tomato_surprised
        #nextdialogue:policeman_suspicious
    }
    #completequest:FindTheDonut

    - else:
    If you find one with the beautiful sprinkles,{br}let me know.
}
