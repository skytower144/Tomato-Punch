VAR isQuestActive = false
VAR isQuestCompleted = false

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
        \* You gave the donut to the policeman.
        #nextdialogue:policeman_fainted
        - else:
        What's that crumb on your face?
        
        oops. I mean, um.... #portrait:tomato_neutral
        
        ...... #hideportrait:_
        
        It's long past my snack time,<br>and all I find is some crumbs on a random kid's face.
        
        Where on earth is my stupid donut?
        
        It's gotta be Bob. I just know it's that greedy buttface.
        
        ........? #portrait:tomato_surprised
        #nextdialogue:policeman_suspicious
    }
    #completequest:FindTheDonut

    - else:
    If you find one, let me know.
}
