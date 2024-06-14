VAR cut = "/cut"
VAR ignore = "/ignore"
VAR br = "<br>"

#teleport:player@-121.05@58
{ignore}

#cutsetcamera:_
#cutmove:player@_~60.5@2@true
#cutmovecamera:_@65.5@2@_@wait
#cutwait:0.5
{cut}

#portrait:Boss_neutral
14 minutes, 46 seconds, 480 milliseconds have passed.

#portrait:Friend_neutral
...is it time to leave work?

#portrait:Boss_neutral
You're late.

Your co-workers have been producing profits for the world,

while also handling your workload.

#portrait:Friend_neutral
I would call it teamwork.

#teleport:Company_SecondFloor_worker_A@-6.2@3.7
#teleport:Company_SecondFloor_worker_B@-5.9@1.7
#teleport:Company_SecondFloor_worker_C@-5.5@-0.1
#teleport:Company_SecondFloor_worker_D@-4.9@-2.2
#teleport:Company_SecondFloor_worker_E@-4.8@-4
#teleport:Company_SecondFloor_worker_F@7.5@3.8
#teleport:Company_SecondFloor_worker_G@7.1@1.8
#teleport:Company_SecondFloor_worker_H@6.7@-0.1
#teleport:Company_SecondFloor_worker_I@6.5@-2
#teleport:Company_SecondFloor_worker_J@6.3@-4
{ignore}

#cutplay:Company_SecondFloor_worker_A@Idle
#cutplay:Company_SecondFloor_worker_B@Idle
#cutplay:Company_SecondFloor_worker_C@Idle
#cutplay:Company_SecondFloor_worker_D@Idle
#cutplay:Company_SecondFloor_worker_E@Idle
#cutplay:Company_SecondFloor_worker_F@Idle
#cutplay:Company_SecondFloor_worker_G@Idle
#cutplay:Company_SecondFloor_worker_H@Idle
#cutplay:Company_SecondFloor_worker_I@Idle
#cutplay:Company_SecondFloor_worker_J@Idle
#cutwait:0.5
{cut}
You know what? I take that back.

#flip:Company_SecondFloor_worker_J
{ignore}

#cutmove:Company_SecondFloor_worker_E@_~59,-124~_,_~63.68,-122.1~_@5@true
#cutmove:Company_SecondFloor_worker_J@_~59,-118~_,_~63.68,-120~_@5@true
{cut}
#portrait:Boss_neutral
For the punishment of your disgusting indolence,

I hereby setence you a head shaving.

#hideportrait:_
#uicanvaslayer:front_ui@3
#movedialoguebox:-220@533
#movecursor:-246@-90
#setboxalpha:0
#textcolor:white
{ignore}

#cutfadeout:0.6@0@4@wait
#cutimage:0@true
#cutfadein:0.6@0@4@wait
{cut}
Wait, what?

#cutfadeout:0.6@0@4@wait
#cutimage:1@true
#cutfadein:0.6@0@4@wait
{cut}
#movecursor:4.17@-125
Wait, wait.{br}Let me explain myself.

#resetdialoguebox:_
#uicanvaslayer:front_ui@3
#portrait:Boss_glasses
Off with his hair.

#hideportrait:_
#uicanvaslayer:front_ui@3
#movedialoguebox:-220@533
#movecursor:4.17@-92
#setboxalpha:0
#textcolor:white
{ignore}

{ignore}
#cutfadeout:0
#cutimage:1@false
{cut}
NOOOOOOO!

#cutplay:Company_SecondFloor_Friend@Friend_HeadShaved
#cutfadein:0.6@0@4@wait
{cut}
#resetdialoguebox:_
#portrait:Boss_neutral
And as for the tomato with silly gloves.

It seems like you are the culprit for spoiling our elite work culture.

#hideportrait:_
\* The boss snaps his fingers.

#flip:Company_SecondFloor_worker_F
#flip:Company_SecondFloor_worker_G
#flip:Company_SecondFloor_worker_H
#flip:Company_SecondFloor_worker_I
{ignore}

#cutturn:player@LEFT-0.5,RIGHT-0.5,LEFT-0.5,RIGHT-0.5,UP-0.5
#cutturn:Company_SecondFloor_worker_E@DOWN
#cutturn:Company_SecondFloor_worker_J@DOWN

#cutmove:Company_SecondFloor_worker_A@_~59,-124~_,_~60.40,-123.25~_@5@true
#cutmove:Company_SecondFloor_worker_B@_~59,-124~_,_~61.75,-123~_,_~61.65@5@true
#cutmove:Company_SecondFloor_worker_C@_~59,-124~_,_~62.55,-122.39~_,_~62.45@5@true
#cutmove:Company_SecondFloor_worker_D@_~59,-124~_,_~62.95,-121.39~_,_~62.85@5@true

#cutmove:Company_SecondFloor_worker_F@_~59,-118~_,_~60.4,-118.59~_@5@true
#cutmove:Company_SecondFloor_worker_G@_~59,-118~_,_~61.75,-118.79~_,_~61.65@5@true
#cutmove:Company_SecondFloor_worker_H@_~59,-118~_,_~62.55,-119.59~_,_~62.45@5@true
#cutmove:Company_SecondFloor_worker_I@_~59,-118~_,_~62.95,-120.41~_,_~62.85@5@true@wait
#cutwait:0.6
{cut}
#portrait:Boss_neutral
You know the drill.

#battletarget:Company_SecondFloor_worker_A
#continuetalk:Company_SecondFloor_ShinyBoss@true
{ignore}