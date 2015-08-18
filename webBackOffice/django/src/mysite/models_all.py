# This is an auto-generated Django model module.
# You'll have to do the following manually to clean this up:
#   * Rearrange models' order
#   * Make sure each model has one field with primary_key=True
#   * Remove `managed = False` lines if you wish to allow Django to create, modify, and delete the table
# Feel free to rename the models, but don't rename db_table values or field names.
#
# Also note: You'll have to insert the output of 'django-admin sqlcustom [app_label]'
# into your database.
from __future__ import unicode_literals

from django.db import models


class Activitymodel(models.Model):
    activityid = models.IntegerField(db_column='activityID', primary_key=True)  # Field name made lowercase.
    begin = models.DateTimeField(blank=True, null=True)
    end = models.DateTimeField(blank=True, null=True)
    parms = models.CharField(max_length=255, blank=True, null=True)

    class Meta:
        managed = False
        db_table = 'activitymodel'


class AuthGroup(models.Model):
    name = models.CharField(unique=True, max_length=80)

    class Meta:
        managed = False
        db_table = 'auth_group'


class AuthGroupPermissions(models.Model):
    group = models.ForeignKey(AuthGroup)
    permission = models.ForeignKey('AuthPermission')

    class Meta:
        managed = False
        db_table = 'auth_group_permissions'
        unique_together = (('group_id', 'permission_id'),)


class AuthPermission(models.Model):
    name = models.CharField(max_length=255)
    content_type = models.ForeignKey('DjangoContentType')
    codename = models.CharField(max_length=100)

    class Meta:
        managed = False
        db_table = 'auth_permission'
        unique_together = (('content_type_id', 'codename'),)


class AuthUser(models.Model):
    password = models.CharField(max_length=128)
    last_login = models.DateTimeField(blank=True, null=True)
    is_superuser = models.IntegerField()
    username = models.CharField(unique=True, max_length=30)
    first_name = models.CharField(max_length=30)
    last_name = models.CharField(max_length=30)
    email = models.CharField(max_length=254)
    is_staff = models.IntegerField()
    is_active = models.IntegerField()
    date_joined = models.DateTimeField()

    class Meta:
        managed = False
        db_table = 'auth_user'


class AuthUserGroups(models.Model):
    user = models.ForeignKey(AuthUser)
    group = models.ForeignKey(AuthGroup)

    class Meta:
        managed = False
        db_table = 'auth_user_groups'
        unique_together = (('user_id', 'group_id'),)


class AuthUserUserPermissions(models.Model):
    user = models.ForeignKey(AuthUser)
    permission = models.ForeignKey(AuthPermission)

    class Meta:
        managed = False
        db_table = 'auth_user_user_permissions'
        unique_together = (('user_id', 'permission_id'),)


class Authority(models.Model):
    id = models.IntegerField(primary_key=True)
    name = models.CharField(max_length=255, blank=True, null=True)
    pwd = models.CharField(max_length=255, blank=True, null=True)
    level = models.IntegerField(blank=True, null=True)

    class Meta:
        managed = False
        db_table = 'authority'


class Blacklistdata(models.Model):
    userid = models.IntegerField(db_column='UserID', primary_key=True)  # Field name made lowercase.
    username = models.CharField(db_column='UserName', max_length=255, blank=True, null=True)  # Field name made lowercase.
    score = models.IntegerField(db_column='Score', blank=True, null=True)  # Field name made lowercase.
    createdate = models.DateTimeField(db_column='CreateDate', blank=True, null=True)  # Field name made lowercase.
    identify = models.CharField(db_column='Identify', max_length=255, blank=True, null=True)  # Field name made lowercase.

    class Meta:
        managed = False
        db_table = 'blacklistdata'


class Compensationinfo(models.Model):
    index = models.IntegerField(primary_key=True)
    type = models.IntegerField(blank=True, null=True)
    message = models.CharField(max_length=255, blank=True, null=True)
    createdate = models.DateTimeField(db_column='CreateDate', blank=True, null=True)  # Field name made lowercase.
    iteminfo = models.CharField(db_column='ItemInfo', max_length=255, blank=True, null=True)  # Field name made lowercase.

    class Meta:
        managed = False
        db_table = 'compensationinfo'


class Datalog(models.Model):
    index = models.IntegerField(db_column='Index', primary_key=True)  # Field name made lowercase.
    userid = models.IntegerField(db_column='UserID', blank=True, null=True)  # Field name made lowercase.
    createdate = models.DateTimeField(db_column='CreateDate', blank=True, null=True)  # Field name made lowercase.
    dimond = models.IntegerField(db_column='Dimond', blank=True, null=True)  # Field name made lowercase.

    class Meta:
        managed = False
        db_table = 'datalog'


class DjangoAdminLog(models.Model):
    action_time = models.DateTimeField()
    object_id = models.TextField(blank=True, null=True)
    object_repr = models.CharField(max_length=200)
    action_flag = models.SmallIntegerField()
    change_message = models.TextField()
    content_type = models.ForeignKey('DjangoContentType', blank=True, null=True)
    user = models.ForeignKey(AuthUser)

    class Meta:
        managed = False
        db_table = 'django_admin_log'


class DjangoContentType(models.Model):
    app_label = models.CharField(max_length=100)
    model = models.CharField(max_length=100)

    class Meta:
        managed = False
        db_table = 'django_content_type'
        unique_together = (('app_label', 'model'),)


class DjangoMigrations(models.Model):
    app = models.CharField(max_length=255)
    name = models.CharField(max_length=255)
    applied = models.DateTimeField()

    class Meta:
        managed = False
        db_table = 'django_migrations'


class DjangoSession(models.Model):
    session_key = models.CharField(primary_key=True, max_length=40)
    session_data = models.TextField()
    expire_date = models.DateTimeField()

    class Meta:
        managed = False
        db_table = 'django_session'


class Exchangecode(models.Model):
    key = models.IntegerField(primary_key=True)
    type = models.IntegerField(db_column='Type', blank=True, null=True)  # Field name made lowercase.
    index = models.IntegerField(db_column='Index', blank=True, null=True)  # Field name made lowercase.
    code = models.CharField(db_column='Code', max_length=255, blank=True, null=True)  # Field name made lowercase.
    cnt = models.IntegerField(blank=True, null=True)
    createdate = models.DateTimeField(db_column='CreateDate', blank=True, null=True)  # Field name made lowercase.

    class Meta:
        managed = False
        db_table = 'exchangecode'


class Gameuser(models.Model):
    userid = models.IntegerField(db_column='UserId', primary_key=True)  # Field name made lowercase.
    nickname = models.CharField(db_column='NickName', max_length=255, blank=True, null=True)  # Field name made lowercase.
    passportid = models.CharField(db_column='PassportId', max_length=255, blank=True, null=True)  # Field name made lowercase.
    retailid = models.CharField(db_column='RetailId', max_length=255, blank=True, null=True)  # Field name made lowercase.
    identify = models.CharField(db_column='Identify', max_length=255, blank=True, null=True)  # Field name made lowercase.
    score = models.IntegerField(db_column='Score', blank=True, null=True)  # Field name made lowercase.
    createtime = models.DateTimeField(db_column='CreateTime', blank=True, null=True)  # Field name made lowercase.
    version = models.CharField(max_length=255, blank=True, null=True)
    state = models.IntegerField(blank=True, null=True)
    diamond = models.IntegerField(db_column='Diamond', blank=True, null=True)  # Field name made lowercase.
    thetotal = models.IntegerField(db_column='theTotal', blank=True, null=True)  # Field name made lowercase.
    preranking = models.IntegerField(db_column='preRanking', blank=True, null=True)  # Field name made lowercase.
    compensationdate = models.DateTimeField(db_column='CompensationDate', blank=True, null=True)  # Field name made lowercase.

    class Meta:
        managed = False
        db_table = 'gameuser'


class Happymodedata(models.Model):
    userid360 = models.IntegerField(db_column='UserId360', primary_key=True)  # Field name made lowercase.
    enternum = models.IntegerField(db_column='EnterNum', blank=True, null=True)  # Field name made lowercase.
    happypoint = models.IntegerField(db_column='HappyPoint', blank=True, null=True)  # Field name made lowercase.
    happyrelivenum = models.IntegerField(db_column='HappyReliveNum', blank=True, null=True)  # Field name made lowercase.
    realitembuycntinrefleshtime = models.TextField(db_column='realItemBuyCntInRefleshTime', blank=True, null=True)  # Field name made lowercase.
    prerefleshtime = models.DateTimeField(db_column='PreRefleshTime', blank=True, null=True)  # Field name made lowercase.
    actionenterhappypoint = models.TextField(db_column='ActionEnterHappyPoint', blank=True, null=True)  # Field name made lowercase.
    realiteminfolst = models.TextField(db_column='RealItemInfoLst', blank=True, null=True)  # Field name made lowercase.
    payinfodic = models.TextField(db_column='PayInfoDic', blank=True, null=True)  # Field name made lowercase.
    the3rduserid = models.IntegerField(db_column='the3rdUserId', blank=True, null=True)  # Field name made lowercase.

    class Meta:
        managed = False
        db_table = 'happymodedata'


class Historyuserranking(models.Model):
    id = models.IntegerField(db_column='ID', primary_key=True)  # Field name made lowercase.
    items = models.TextField(db_column='Items', blank=True, null=True)  # Field name made lowercase.

    class Meta:
        managed = False
        db_table = 'historyuserranking'


class LearnAuthor(models.Model):
    first_name = models.CharField(max_length=30)
    last_name = models.CharField(max_length=40)
    email = models.CharField(max_length=254)

    class Meta:
        managed = False
        db_table = 'learn_author'


class LearnBook(models.Model):
    title = models.CharField(max_length=100)
    publisher = models.ForeignKey('LearnPublisher')
    publication_date = models.DateField()

    class Meta:
        managed = False
        db_table = 'learn_book'


class LearnBookAuthors(models.Model):
    book = models.ForeignKey(LearnBook)
    author = models.ForeignKey(LearnAuthor)

    class Meta:
        managed = False
        db_table = 'learn_book_authors'
        unique_together = (('book_id', 'author_id'),)


class LearnPublisher(models.Model):
    name = models.CharField(max_length=30)
    address = models.CharField(max_length=50)
    city = models.CharField(max_length=60)
    state_province = models.CharField(max_length=30)
    country = models.CharField(max_length=50)
    website = models.CharField(max_length=200)

    class Meta:
        managed = False
        db_table = 'learn_publisher'


class Pay(models.Model):
    userid = models.IntegerField(db_column='UserId', primary_key=True)  # Field name made lowercase.
    identify = models.CharField(db_column='Identify', max_length=255, blank=True, null=True)  # Field name made lowercase.
    serverorderid = models.CharField(db_column='ServerOrderId', max_length=255, blank=True, null=True)  # Field name made lowercase.
    createdate = models.DateTimeField(db_column='CreateDate', blank=True, null=True)  # Field name made lowercase.
    productid = models.CharField(db_column='ProductId', max_length=255, blank=True, null=True)  # Field name made lowercase.
    num = models.IntegerField(blank=True, null=True)
    payinfo = models.CharField(db_column='PayInfo', max_length=255, blank=True, null=True)  # Field name made lowercase.
    state = models.IntegerField(blank=True, null=True)
    process = models.TextField(blank=True, null=True)  # This field type is a guess.
    hasgetpayreward = models.TextField(db_column='hasGetPayReward', blank=True, null=True)  # Field name made lowercase. This field type is a guess.
    index = models.IntegerField(db_column='Index', blank=True, null=True)  # Field name made lowercase.

    class Meta:
        managed = False
        db_table = 'pay'


class Payorder(models.Model):
    index = models.IntegerField(db_column='Index', primary_key=True)  # Field name made lowercase.
    userid = models.IntegerField(db_column='UserId', blank=True, null=True)  # Field name made lowercase.
    identify = models.CharField(db_column='Identify', max_length=255, blank=True, null=True)  # Field name made lowercase.
    serverorderid = models.CharField(db_column='ServerOrderId', max_length=255, blank=True, null=True)  # Field name made lowercase.
    createdate = models.DateTimeField(db_column='CreateDate', blank=True, null=True)  # Field name made lowercase.
    productid = models.CharField(db_column='ProductId', max_length=255, blank=True, null=True)  # Field name made lowercase.
    num = models.IntegerField(blank=True, null=True)
    payinfo = models.CharField(db_column='PayInfo', max_length=255, blank=True, null=True)  # Field name made lowercase.
    state = models.IntegerField(blank=True, null=True)
    process = models.TextField(blank=True, null=True)  # This field type is a guess.
    hasgetpayreward = models.TextField(db_column='hasGetPayReward', blank=True, null=True)  # Field name made lowercase. This field type is a guess.
    the3rdorder = models.IntegerField(db_column='the3rdOrder', blank=True, null=True)  # Field name made lowercase.
    typeuser = models.CharField(db_column='typeUser', max_length=255, blank=True, null=True)  # Field name made lowercase.
    userparms = models.CharField(db_column='userParms', max_length=255, blank=True, null=True)  # Field name made lowercase.
    the3rduserid = models.IntegerField(db_column='the3rdUserId', blank=True, null=True)  # Field name made lowercase.
    the3rdorderid = models.CharField(db_column='the3rdOrderId', max_length=255, blank=True, null=True)  # Field name made lowercase.
    strthe3rdorderid = models.CharField(db_column='strThe3rdOrderId', max_length=255, blank=True, null=True)  # Field name made lowercase.

    class Meta:
        managed = False
        db_table = 'payorder'


class Payuserinfoex(models.Model):
    the3rdusrid = models.IntegerField(db_column='the3rdUsrID', primary_key=True)  # Field name made lowercase.
    the3rdusrname = models.CharField(db_column='the3rdUsrName', max_length=255, blank=True, null=True)  # Field name made lowercase.
    userid = models.IntegerField(db_column='UserId', blank=True, null=True)  # Field name made lowercase.
    identify = models.CharField(max_length=255, blank=True, null=True)
    createdate = models.DateTimeField(db_column='CreateDate', blank=True, null=True)  # Field name made lowercase.
    infoext = models.CharField(db_column='InfoExt', max_length=255, blank=True, null=True)  # Field name made lowercase.
    typeuser = models.CharField(db_column='typeUser', max_length=255, blank=True, null=True)  # Field name made lowercase.
    for360userid = models.IntegerField(db_column='for360UserId', blank=True, null=True)  # Field name made lowercase.

    class Meta:
        managed = False
        db_table = 'payuserinfoex'


class Realiteminfo(models.Model):
    index = models.IntegerField(db_column='Index', primary_key=True)  # Field name made lowercase.
    userid = models.IntegerField(db_column='UserId', blank=True, null=True)  # Field name made lowercase.
    identify = models.CharField(db_column='Identify', max_length=255, blank=True, null=True)  # Field name made lowercase.
    the3rduserid = models.IntegerField(db_column='the3rdUserId', blank=True, null=True)  # Field name made lowercase.
    createdate = models.DateTimeField(db_column='CreateDate', blank=True, null=True)  # Field name made lowercase.
    realitemid = models.IntegerField(db_column='realItemID', blank=True, null=True)  # Field name made lowercase.
    extinfo = models.CharField(db_column='extInfo', max_length=255, blank=True, null=True)  # Field name made lowercase.
    state = models.IntegerField(blank=True, null=True)
    happypoint = models.IntegerField(db_column='happyPoint', blank=True, null=True)  # Field name made lowercase.
    needhappypoint = models.IntegerField(db_column='needHappyPoint', blank=True, null=True)  # Field name made lowercase.

    class Meta:
        managed = False
        db_table = 'realiteminfo'


class Realuserinfo(models.Model):
    userid360 = models.IntegerField(db_column='UserId360', primary_key=True)  # Field name made lowercase.
    name = models.CharField(max_length=255, blank=True, null=True)
    phonenum = models.CharField(db_column='PhoneNum', max_length=255, blank=True, null=True)  # Field name made lowercase.
    address = models.CharField(max_length=255, blank=True, null=True)
    the3rduseri = models.IntegerField(db_column='the3rdUserI', blank=True, null=True)  # Field name made lowercase.
    for360userid = models.IntegerField(db_column='for360UserId', blank=True, null=True)  # Field name made lowercase.

    class Meta:
        managed = False
        db_table = 'realuserinfo'


class Serverinfomode(models.Model):
    id = models.IntegerField(primary_key=True)
    type = models.IntegerField(blank=True, null=True)
    ip = models.CharField(max_length=255, blank=True, null=True)
    ipuint = models.IntegerField(db_column='ipUint', blank=True, null=True)  # Field name made lowercase.
    wight = models.IntegerField(blank=True, null=True)
    offlinecnt = models.IntegerField(db_column='offLineCnt', blank=True, null=True)  # Field name made lowercase.
    userd = models.TextField(blank=True, null=True)  # This field type is a guess.
    status = models.IntegerField(blank=True, null=True)
    port = models.IntegerField(blank=True, null=True)

    class Meta:
        managed = False
        db_table = 'serverinfomode'


class Sharerealitemcnt(models.Model):
    itemid = models.IntegerField(db_column='itemID', primary_key=True)  # Field name made lowercase.
    num = models.IntegerField(blank=True, null=True)
    preupdatetime = models.DateTimeField(db_column='preUpdateTime', blank=True, null=True)  # Field name made lowercase.

    class Meta:
        managed = False
        db_table = 'sharerealitemcnt'


class Sharerealiteminfo(models.Model):
    index = models.IntegerField(db_column='Index', primary_key=True)  # Field name made lowercase.
    userid = models.IntegerField(db_column='UserId', blank=True, null=True)  # Field name made lowercase.
    identify = models.CharField(db_column='Identify', max_length=255, blank=True, null=True)  # Field name made lowercase.
    the3rduserid = models.IntegerField(db_column='the3rdUserId', blank=True, null=True)  # Field name made lowercase.
    createdate = models.DateTimeField(db_column='CreateDate', blank=True, null=True)  # Field name made lowercase.
    realitemid = models.IntegerField(db_column='realItemID', blank=True, null=True)  # Field name made lowercase.
    extinfo = models.CharField(db_column='extInfo', max_length=255, blank=True, null=True)  # Field name made lowercase.
    state = models.IntegerField(blank=True, null=True)
    happypoint = models.IntegerField(db_column='happyPoint', blank=True, null=True)  # Field name made lowercase.
    needhappypoint = models.IntegerField(db_column='needHappyPoint', blank=True, null=True)  # Field name made lowercase.

    class Meta:
        managed = False
        db_table = 'sharerealiteminfo'


class TempEntityhistory(models.Model):
    key = models.CharField(db_column='Key', primary_key=True, max_length=100)  # Field name made lowercase.
    value = models.TextField(db_column='Value', blank=True, null=True)  # Field name made lowercase.

    class Meta:
        managed = False
        db_table = 'temp_entityhistory'


class Testupdate(models.Model):
    index = models.IntegerField(primary_key=True)
    itemid = models.IntegerField(db_column='itemID', blank=True, null=True)  # Field name made lowercase.

    class Meta:
        managed = False
        db_table = 'testupdate'


class The3Rduseridmap(models.Model):
    index = models.IntegerField(db_column='Index', primary_key=True)  # Field name made lowercase.
    the3rdmap = models.TextField(db_column='the3rdMap', blank=True, null=True)  # Field name made lowercase.

    class Meta:
        managed = False
        db_table = 'the3rduseridmap'


class Useridmap(models.Model):
    userid = models.IntegerField(db_column='UserID', primary_key=True)  # Field name made lowercase.

    class Meta:
        managed = False
        db_table = 'useridmap'


class Userranking(models.Model):
    userid = models.IntegerField(db_column='UserID', primary_key=True)  # Field name made lowercase.
    username = models.CharField(db_column='UserName', max_length=255, blank=True, null=True)  # Field name made lowercase.
    score = models.IntegerField(db_column='Score', blank=True, null=True)  # Field name made lowercase.
    createdate = models.DateTimeField(db_column='CreateDate', blank=True, null=True)  # Field name made lowercase.
    identify = models.CharField(db_column='Identify', max_length=255, blank=True, null=True)  # Field name made lowercase.

    class Meta:
        managed = False
        db_table = 'userranking'


class Userrankingtotal(models.Model):
    userid = models.IntegerField(db_column='UserID', primary_key=True)  # Field name made lowercase.
    total = models.IntegerField(db_column='Total', blank=True, null=True)  # Field name made lowercase.
    preranking = models.IntegerField(db_column='preRanking', blank=True, null=True)  # Field name made lowercase.

    class Meta:
        managed = False
        db_table = 'userrankingtotal'
