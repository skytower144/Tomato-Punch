VAR isQuestCompleted = false

#hideportrait:_
#checkquest:LockedDoor_House4
\* The door seems to be locked. 
{
    - isQuestCompleted:
    #completequest:LockedDoor_House4
    You used the rusty key to open the door. #unlockportal:_
    The door opened.
    
    - else:
    I... can't quite remember who lived here. #portrait:tomato_neutral
}