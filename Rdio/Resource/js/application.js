
Rdio = {
    'Tools': {
        'Selectize': function (object, type, create) {
            if (create == undefined)
                create = true;
            if (type == 'MultiSelect') {
                $(object).selectize({
                    plugins: ['remove_button'],
                    persist: false,
                    create: create
                });
            }
            
        },
        'Pagination': {
            'Siteid': '',
            'BaseContentSearchModel': '',
            'ContentSearchModel': '',
            'Event': function (type, page) {
                $('body').on('click', '.paginatopnbtn', function () {
                    var pagenumber = $(this).data('pagenumber');
                    Rdio.Tools.Pagination.GetPagination(type,pagenumber);
                });

                if (type == 'SiteManage') {
                    Rdio.Tools.Pagination.GetPagination(type, 1);
                }

                if (type == 'RssManage') {
                    Rdio.Tools.Pagination.Siteid = $('[name="siteid"]').val();
                    Rdio.Tools.Pagination.GetPagination(type, 1);
                }

                if (type == 'BaseContentManage') {
                    var model = Rdio.BaseContent.BaseContentSearchModel;
                    model.page = page;
                    model.rssid = $('[name="rssid"]').val();
                    model.siteid = $('[name="siteid"]').val();
                    model.tags = $('[name="tags"]').val();
                    model.categories = $('[name="categories"]').val();
                    Rdio.Tools.Pagination.BaseContentSearchModel = model;
                    Rdio.Tools.Pagination.GetPagination(type, page);
                }

                if (type == 'ContentManage') {
                    var model = Rdio.Content.ContentSearchModel;
                    model.page = page;
                    model.rssid = $('[name="rssid"]').val();
                    model.siteid = $('[name="siteid"]').val();
                    model.tags = $('[name="tags"]').val();
                    model.categories = $('[name="categories"]').val();
                    Rdio.Tools.Pagination.ContentSearchModel = model;
                    Rdio.Tools.Pagination.GetPagination(type, page);
                }

                if (type == 'CategoryManage') {
                    Rdio.Tools.Pagination.GetPagination(type, 1);
                }

            },
            'GetPagination': function (type, page) {

                if (type == 'SiteManage') {
                    loader($('#datacontainer'));
                    $.ajax({
                        method: "GET",
                        url: "/api/ContentManager/SiteManage",
                        data: { "page": page }
                    }).done(function (result) {
                        disloader($('#datacontainer'));

                        if (result.ServiceResultStatus == 0) {
                            var source = $("#datamodel").html();
                            var template = Handlebars.compile(source);
                            var html = template(result);
                            $("#datacontainer").html(html);
                        }
                        if (result.ServiceResultStatus == 1) {
                            alert(result.ServiceResultMassage);
                        }
                    });
                }

                if (type == 'RssManage') {
                    loader($('#datacontainer'));
                    $.ajax({
                        method: "GET",
                        url: "/api/ContentManager/RssManage",
                        data: { "page": page, "siteid": Rdio.Tools.Pagination.Siteid }
                    }).done(function (result) {
                        disloader($('#datacontainer'));

                        if (result.ServiceResultStatus == 0) {
                            var source = $("#datamodel").html();
                            var template = Handlebars.compile(source);
                            var html = template(result);
                            $("#datacontainer").html(html);
                        }
                        if (result.ServiceResultStatus == 1) {
                            alert(result.ServiceResultMassage);
                        }
                    });
                }

                if (type == 'BaseContentManage') {
                    loader($('#datacontainer'));
                    Rdio.Tools.Pagination.BaseContentSearchModel.page = page;
                    $.ajax({
                        method: "POST",
                        url: "/api/BaseContent/Manage",
                        data: Rdio.Tools.Pagination.BaseContentSearchModel
                    }).done(function (result) {
                        disloader($('#datacontainer'));

                        if (result.ServiceResultStatus == 0) {
                            var source = $("#datamodel").html();
                            var template = Handlebars.compile(source);
                            var html = template(result);
                            $("#datacontainer").html(html);
                        }
                        if (result.ServiceResultStatus == 1) {
                            alert(result.ServiceResultMassage);
                        }
                    });
                }

                if (type == 'ContentManage') {
                    loader($('#datacontainer'));
                    Rdio.Tools.Pagination.ContentSearchModel.page = page;
                    $.ajax({
                        method: "POST",
                        url: "/api/Content/Manage",
                        data: Rdio.Tools.Pagination.ContentSearchModel
                    }).done(function (result) {
                        disloader($('#datacontainer'));

                        if (result.ServiceResultStatus == 0) {
                            var source = $("#datamodel").html();
                            var template = Handlebars.compile(source);
                            var html = template(result);
                            $("#datacontainer").html(html);
                        }
                        if (result.ServiceResultStatus == 1) {
                            alert(result.ServiceResultMassage);
                        }
                    });
                }

                if (type == 'CategoryManage') {
                    loader($('#datacontainer'));
                    $.ajax({
                        method: "GET",
                        url: "/api/ContentManager/CategoryManage",
                        data: { "page": page}
                    }).done(function (result) {
                        disloader($('#datacontainer'));

                        if (result.ServiceResultStatus == 0) {
                            var source = $("#datamodel").html();
                            var template = Handlebars.compile(source);
                            var html = template(result);
                            $("#datacontainer").html(html);
                        }
                        if (result.ServiceResultStatus == 1) {
                            alert(result.ServiceResultMassage);
                        }
                    });
                }
            }
        },
        'Confirm':function(message) {
            var confirmR = confirm("مورد مورد نظر حذف گردد؟");
            return confirmR;
        },
        'PushMessage':function(message, type) {
            alert(message);
        }
    },
    'ContentManager': {
        'EditSiteModel': { '_id': '', 'title': '', 'url': '' },
        'EditRssModel': { '_id': '', 'siteid': '', 'title': '', 'url': '', 'tags': '', 'categories': '', 'lang': '' },
        'EditTemplateModel': { '_id': '', 'siteid': '', 'name': '', 'type': '', 'sampleurl': '', 'structure': '' },
        'EditTemplateStructureModel': { 'field': '', 'query': '', 'type': '' },
        'DeleteSiteModel': { 'id': '' },
        'EditCategoryModel': { '_id': '','userid':'', 'title': '', 'parentid': '', 'blocks': ''},

        'Event':{
            'SiteManage':function(){
                Rdio.Tools.Pagination.Event('SiteManage', 1);
                $('body').on('click', '.DeleteSite', function () {
                    siteId = $(this).attr('data-siteid');
                    objhtml = $(this);
                    var confirmR = Rdio.Tools.Confirm();
                    var model = Rdio.ContentManager.DeleteSiteModel;
                    model.id = siteId;
                    if (confirmR == true) {
                        $.ajax({
                            method: "POST",
                            url: "/api/ContentManager/DeleteSite",
                            data: model
                        }).done(function (result) {
                            if (result.ServiceResultStatus == 0) {
                                $(objhtml).closest('tr').remove();
                            }
                            if (result.ServiceResultStatus == 1) {
                                Rdio.Tools.PushMessage(result.ServiceResultMassage);
                            }
                        }).fail(function (result) {
                            Rdio.Tools.PushMessage(result.responseText);
                        });
                    }
                    
                });

            },
            'EditSite': function () {

                $('body').on('click', '#editsitesubmit', function () {
                    var editmodel = Rdio.ContentManager.EditSiteModel;
                    editmodel.title = $('#title').val();
                    editmodel.url = $('#url').val();
                    editmodel._id = $('[name="_id"]').val();

                    $.ajax({
                        method: "POST",
                        url: "/api/ContentManager/EditSite",
                        data: editmodel
                    }).done(function (result) {
                        if (result.ServiceResultStatus == 0) {
                            alert(result.ServiceResultMassage);
                            window.location = '/ContentManager/SiteManage';
                        }
                        if (result.ServiceResultStatus == 1) {
                            alert(result.ServiceResultMassage);
                        }
                    });
                });
            },
            'RssManage': function () {
                Rdio.Tools.Pagination.Event('RssManage', 1);
                $('body').on('click', '.DeleteRss', function () {
                    rssId = $(this).attr('data-rssid');
                    objhtml = $(this);
                    var confirmR = Rdio.Tools.Confirm();
                    //var model = Rdio.ContentManager.DeleteSiteModel;
                    //model.id = siteId;
                    if (confirmR == true) {
                        $.ajax({
                            method: "POST",
                            url: "/api/ContentManager/DeleteRss",
                            data: { 'id': rssId }
                        }).done(function (result) {
                            if (result.ServiceResultStatus == 0) {
                                $(objhtml).closest('tr').remove();
                            }
                            if (result.ServiceResultStatus == 1) {
                                Rdio.Tools.PushMessage(result.ServiceResultMassage);
                            }
                        }).fail(function (result) {
                            Rdio.Tools.PushMessage(result.responseText);
                        });
                    }

                });

            },
            'EditRss': function () {
                Rdio.Tools.Selectize($('#tags'), 'MultiSelect');
                Rdio.Tools.Selectize($('#categories'), 'MultiSelect');

                $('body').on('click', '#editrsssubmit', function () {
                    var editmodel = Rdio.ContentManager.EditRssModel;
                    editmodel._id = $('[name="_id"]').val();
                    editmodel.siteid = $('[name="siteid"]').val();
                    editmodel.title = $('#title').val();
                    editmodel.url = $('#url').val();
                    editmodel.tags = $('#tags').val().split(',');
                    editmodel.categories = $('#categories').val().split(',');
                    editmodel.lang = $('#lang').val();
                    $.ajax({
                        method: "POST",
                        url: "/api/ContentManager/EditRss",
                        data: editmodel
                    }).done(function (result) {
                        if (result.ServiceResultStatus == 0) {
                            alert(result.ServiceResultMassage);
                            window.location = '/ContentManager/RssManage?siteid=' + editmodel.siteid;
                        }
                        if (result.ServiceResultStatus == 1) {
                            alert(result.ServiceResultMassage);
                        }
                    });
                });
            },
            'TemplateManage': function () {
                //Rdio.Tools.Pagination.Event('TemplateManage', 1);

            },
            'EditTemplate': function () {

                $('#showsampleurlmodal').on('show.bs.modal', function (event) {
                    var button = $(event.relatedTarget);
                    var modal = $(this);
                    modal.find('.modal-body #sampleurliframe').attr('src', $('#sampleurl').val());
                });

                $('body').on('click', '#showsampleurl', function () {
                    $('#showsampleurlmodal').modal('show');
                });

                $('body').on('change', '#type', function () {
                    $('.contenttemplate').hide();
                    $('#contenttemplate_' + $(this).val()).show();
                });

                $('body').on('click', '#edittemplatesubmit', function () {
                    var editmodel = Rdio.ContentManager.EditTemplateModel;
                    editmodel._id = $('[name="_id"]').val();
                    editmodel.siteid = $('[name="siteid"]').val();
                    editmodel.name = $('#name').val();
                    editmodel.sampleurl = $('#sampleurl').val();
                    editmodel.type = $('#type').val();
                    editmodel.structure = [];
                    $('#TemplateStructures .contenttemplate:visible .form-group').each(function (i, v) {
                        var structurModel = {};//Rdio.ContentManager.EditTemplateStructureModel;}
                        structurModel.field = $(this).find("[data-field]").attr("data-field");
                        structurModel.query = $(this).find("[data-field]").val();
                        structurModel.type = $(this).find("#structure_type").val();
                        editmodel.structure.push(structurModel);
                    });

                    
                    $.ajax({
                        method: "POST",
                        url: "/api/ContentManager/EditTemplate",
                        data: editmodel
                    }).done(function (result) {
                        if (result.ServiceResultStatus == 0) {
                            alert(result.ServiceResultMassage);
                            //window.location = '/ContentManager/RssManage?siteid=' + editmodel.siteid;
                        }
                        if (result.ServiceResultStatus == 1) {
                            alert(result.ServiceResultMassage);
                        }
                    });
                });
            },
            'CategoryManage': function () {
                Rdio.Tools.Pagination.Event('CategoryManage', 1);
            },
            'EditCategory': function () {
                Rdio.Tools.Selectize($('#parentid'), 'MultiSelect', false);
                Rdio.Tools.Selectize($('#blocks'), 'MultiSelect',false);

                $('body').on('click', '#editcategorysubmit', function () {
                    var editmodel = Rdio.ContentManager.EditCategoryModel;
                    editmodel._id = $('[name="_id"]').val();
                    editmodel.userid = $('[name="userid"]').val();
                    editmodel.title = $('#title').val();
                    editmodel.parentid = $('#parentid').val();
                    editmodel.blocks = [];

                    $($('#blocks').val()).each(function (i, v) {
                        var blockModel = {};
                        blockModel.title = v.split('|')[0];
                        blockModel.code = v.split('|')[1];
                        editmodel.blocks.push(blockModel);
                    });

                    $.ajax({
                        method: "POST",
                        url: "/api/ContentManager/EditCategory",
                        data: editmodel
                    }).done(function (result) {
                        if (result.ServiceResultStatus == 0) {
                            alert(result.ServiceResultMassage);
                            window.location = '/ContentManager/CategoryManage';
                        }
                        if (result.ServiceResultStatus == 1) {
                            alert(result.ServiceResultMassage);
                        }
                    });
                });
            },
            'BlockRssBindManager': function () {

                $('body').on('click', '#addblockrssbind', function () {
                    var rssid = $('#rssid').val();
                    var rssTitle = $('#rssid option:selected').text();

                    var categoryid = $('#categoryidblockcode').val().split('|')[0];
                    var blockCode = $('#categoryidblockcode').val().split('|')[1];
                    var CategoryBlockTitle = $('#categoryidblockcode option:selected').text();

                    var CatIdBCodeRssId = categoryid + "|" + blockCode + "|" + rssid;

                    //Prevent Repeated Data
                    var IsRepeatedData = false;
                    $('#blockrssbindcontainer .BlockRssBindItem').each(function(i, v) {
                        if ($(this).attr('data-CatIdBCodeRssId') == CatIdBCodeRssId) {
                            Rdio.Tools.PushMessage("این رابطه در لیست وجود دارد.");
                            IsRepeatedData = true;
                            return false;
                        }
                    });

                    if (!IsRepeatedData) {
                        var source = $("#blockrssbindtemplatemodel").html();
                        var template = Handlebars.compile(source);
                        var result = { 'Title': CategoryBlockTitle + ' -> ' + rssTitle, 'CatIdBCodeRssId': CatIdBCodeRssId };
                        var html = template(result);
                        $("#blockrssbindcontainer").append(html);
                    }
                    
                });
                $('body').on('click', '.RemoveBlockRssBindItem', function () {
                    $(this).closest('.BlockRssBindItem').remove();
                });
                $('body').on('click', '#SaveBlockRssBind', function () {
                    var BlockRssBind = [];
                    $('#blockrssbindcontainer .BlockRssBindItem').each(function (i, v) {
                        var CatIdBCodeRssId = $(this).attr('data-catidbcoderssid');
                        var CategoryId = CatIdBCodeRssId.split('|')[0];
                        var BlockCode = CatIdBCodeRssId.split('|')[1];
                        var RssId = CatIdBCodeRssId.split('|')[2];
                        BlockRssBind.push({ 'CategoryId': CategoryId, 'BlockCode': BlockCode, 'RssId': RssId });
                    });

                    $.ajax({
                        method: "POST",
                        url: "/api/ContentManager/EditBlockRssBind",
                        data: {'_id':$('[name="_id"]').val(),'BlockRssBind':BlockRssBind}
                    }).done(function (result) {
                        if (result.ServiceResultStatus == 0) {
                            alert(result.ServiceResultMassage);
                            window.location = '/ContentManager/BlockRssBindManage';
                        }
                        if (result.ServiceResultStatus == 1) {
                            alert(result.ServiceResultMassage);
                        }
                    });

                });
            }
            }
    },
    'BaseContent': {
        'BaseContentSearchModel': { 'page': 1, 'rssid': '', 'tags': '', 'categories': '', 'siteid': '' },

        'Event': {
            'Manage': function () {

                $('body').on('click', '#basecontentsearchsubmit', function () {
                    Rdio.Tools.Pagination.Event('BaseContentManage', 1);
                });

                Rdio.Tools.Pagination.Event('BaseContentManage', 1);
            }
        }
    },
    'Content': {
        'ContentSearchModel': { 'page': 1, 'rssid': '', 'tags': '', 'categories': '', 'siteid': '' },

        'Event': {
            'Manage': function () {

                $('body').on('click', '#basecontentsearchsubmit', function () {
                    Rdio.Tools.Pagination.Event('BaseContentManage', 1);
                });

                Rdio.Tools.Pagination.Event('ContentManage', 1);
            }
        }
    },
    'Admin': {
        'Event': {
            'M': function () {

            }
        }
    }
    
};
$(document).ready(function () {
    handelbarfunchelperinit();

    //Rdio.ContentManager.Event.EditSite();
    //Rdio.ContentManager.Event.SiteManage();
    //Rdio.ContentManager.Event.RssManage();
    //Rdio.ContentManager.Event.EditRss();

});

