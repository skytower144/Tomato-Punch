VAR cut = "/cut"
VAR ignore = "/ignore"
VAR br = "<br>"

#cutsetcamera:_
#cutmove:player@_~60.5@2@true
#cutmovecamera:_@65.5@2@_@wait
#cutwait:0.5
{cut}

#portrait:Boss_neutral
14 minutes, 46 seconds, 480 milliseconds have passed.

#portrait:Friend_neutral
...Is it time to leave work?

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

#cutmove:Company_SecondFloor_worker_E@_~59,-124~_,_~63.68,-123.99~_@5@true
#cutmove:Company_SecondFloor_worker_J@_~59,-118~_,_~63.68,-118.01~_@5@true
{cut}
#portrait:Boss_neutral
For the punishment of your disgusting indolence,

I hereby setence you a head shaving.

